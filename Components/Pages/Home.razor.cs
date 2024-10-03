
using CreditReportViewer.Models;
using CreditReportViewer.MVC;

using Microsoft.AspNetCore.Components;

namespace CreditReportViewer.Components.Pages;

public partial class Home : ICreditReportsView
{
    private CreditReportController? _controller;
    private Guid selectedUid;
    private DataModel? data;

    [Inject]
    public required ILogger<Home> Logger
    {
        get;
        set;
    }

    [Inject]
    public required CreditReportController? Controller
    {
        get => _controller;
        set
        {
            if (_controller is { })
            {
                _controller.DataLoaded -= _controller_DataLoaded;
                _controller.DataFailed -= _controller_DataFailed;
            }

            _controller = value;

            if (_controller is { })
            {
                _controller.DataLoaded -= _controller_DataLoaded;
                _controller.DataFailed -= _controller_DataFailed;
                _controller.DataLoaded += _controller_DataLoaded;
                _controller.DataFailed += _controller_DataFailed;
            }
        }
    }

    private void _controller_DataFailed(object? sender, Exception? e)
    {
        Logger.LogError(e, "Failed to load credit reports.");
        StateHasChanged();
    }

    private void _controller_DataLoaded(object? sender, EventArgs e)
    {
        Logger.LogInformation("Data loaded with {Count} records.", Count);
        StateHasChanged();
    }

    public Guid SelectedUid
    {
        get => selectedUid;
        set
        {
            selectedUid = value;

            Logger.LogDebug("Set Selection to {UID}", selectedUid);
        }
    }
    private int Count => Data?.CreditReports.Length ?? 0;

    public DataModel? Data
    {
        get => data;
        set
        {
            data = value;
            CreditReports = data?.CreditReports.AsQueryable();
        }
    }

    public IQueryable<CreditReport>? CreditReports
    {
        get;
        set;
    }

    public IList<CreditReport>? SelectedCreditReports
    {
        get;
        set;
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if (Controller is { })
        {
            await Controller.LoadDataAsync(this);
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender && Controller is { } && Data is { })
        {
            await Controller.SaveDataAsync(this);
        }
    }

    public void ViewReport(object args)
    {
        if (args is Guid uid)
        {
            SelectedUid = uid;
            Controller?.ViewReport(SelectedUid);
        }
    }

}
