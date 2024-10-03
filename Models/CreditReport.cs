namespace CreditReportViewer.Models;

public class CreditReport
{
    public string? NameFirst
    {
        get; set;
    }
    public string? NameLast
    {
        get; set;
    }
    public string? CreditReportName
    {
        get; set;
    }
    public int? CreditScore
    {
        get; set;
    }
    public required Guid UID
    {
        get; set;
    } = Guid.NewGuid();

    public IList<string> Aliases
    {
        get; set;
    } = [];
}
