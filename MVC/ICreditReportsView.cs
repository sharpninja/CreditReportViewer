using CreditReportViewer.Models;

namespace CreditReportViewer.MVC;

public interface ICreditReportsView
{
    DataModel? Data
    {
        get; set;
    }

    Guid SelectedUid
    {
        get; set;
    }

    IList<CreditReport>? SelectedCreditReports
    {
        get;
        set;
    }
}

public interface ICreditReportDetailsView
{
    CreditReport? CreditReport
    {
        get;
        set;
    }
}