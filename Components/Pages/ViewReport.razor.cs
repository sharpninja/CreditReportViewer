
using CreditReportViewer.Models;
using CreditReportViewer.MVC;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;

namespace CreditReportViewer.Components.Pages;

public partial class ViewReport : ICreditReportDetailsView
{
    private CreditReportController? _controller;

    [Inject]
    public required ILogger<ViewReport> Logger
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
        Logger.LogInformation("Data loaded .");
        StateHasChanged();
    }

    public CreditReport? CreditReport
    {
        get;
        set;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        try
        {
            await base.OnAfterRenderAsync(firstRender);

            if (Controller is { } && firstRender)
            {
                await Controller.GetSelectedCreditReport(this);
            }
        }
        catch
        {
            // Do nothing.  Exists for breakpoint while debugging.
        }
    }
}
