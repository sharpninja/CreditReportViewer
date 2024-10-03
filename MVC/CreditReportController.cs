
using CreditReportViewer.Components.Pages;
using CreditReportViewer.Models;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CreditReportViewer.MVC;

public class CreditReportController(
    ILogger<CreditReportController> logger,
    NavigationManager navManager,
    ProtectedSessionStorage protectedSessionStore)
{
    protected DataModel? LoadedDataModel
    {
        get;
        set;
    }

    public Guid CurrentReportUid
    {
        get;
        set;
    }

    public async Task LoadDataAsync(ICreditReportsView view)
    {
        try
        {
            string filename = Path.Combine("Data", "data.json");
            FileInfo file = new(filename);
            if (file.Exists)
            {
                using StreamReader reader = new(file.OpenRead());
                string json = await reader.ReadToEndAsync();
                view.Data = JsonConvert.DeserializeObject<DataModel>(json) ?? new()
                {
                    CreditReports = []
                };
                DataLoaded?.Invoke(this, EventArgs.Empty);
                logger.LogInformation("Loaded {FileName}", filename);
            }
            else
            {
                logger.LogError("{FileName} was not found.", filename);
                throw new FileNotFoundException(filename);
            }
        }
        catch (Exception ex)
        {
            DataFailed?.Invoke(this, ex);
        }
    }

    public async Task GetSelectedCreditReport(ICreditReportDetailsView view)
    {
        try
        {
            LoadedDataModel = (await protectedSessionStore.GetAsync<DataModel>(nameof(DataModel))).Value;
            CurrentReportUid = (await protectedSessionStore.GetAsync<Guid>(nameof(CurrentReportUid))).Value;
            if (CurrentReportUid != default)
            {
                view.CreditReport = LoadedDataModel?.CreditReports.FirstOrDefault(cr => cr.UID == CurrentReportUid);
                DataLoaded?.Invoke(this, EventArgs.Empty);
            }
        }
        catch (Exception ex)
        {
            DataFailed?.Invoke(this, ex);
        }
    }

    public async Task ViewReport(Guid selectedUid)
    {
        await protectedSessionStore.SetAsync(nameof(CurrentReportUid), selectedUid);
        navManager.NavigateTo($"ViewReport");
    }

    internal async Task SaveDataAsync(ICreditReportsView view)
    {
        logger.LogInformation("Entering SaveDataAsync");
        await protectedSessionStore.SetAsync(nameof(DataModel), view.Data!);
        LoadedDataModel ??= (await protectedSessionStore.GetAsync<DataModel>(nameof(DataModel))).Value;
        if (LoadedDataModel is { })
        {
            view.Data = LoadedDataModel;
            CurrentReportUid = (await protectedSessionStore.GetAsync<Guid>(nameof(CurrentReportUid))).Value;
            if (CurrentReportUid != default)
            {
                view.SelectedCreditReports = LoadedDataModel.CreditReports.Where(cr => cr.UID == CurrentReportUid).ToList();
                DataLoaded?.Invoke(this, EventArgs.Empty);
            }
        }
        logger.LogInformation("Exiting SaveDataAsync");

    }

    public event EventHandler? DataLoaded;
    public event EventHandler<Exception?>? DataFailed;
}
