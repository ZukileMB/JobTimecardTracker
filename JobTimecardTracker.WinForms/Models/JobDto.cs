
namespace JobTimecardTracker.WinForms.Models;

public class JobDto
{
    public Guid Id { get; set; }
    public string JobNumber { get; set; } = string.Empty;
    public JobType JobType { get; set; }
    public DateTime JobDate { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string ContactName { get; set; } = string.Empty;
    public string TelephoneNumber { get; set; } = string.Empty;

    public string DisplaySummary => $"{JobNumber} - {CompanyName} ({JobType})";
}
