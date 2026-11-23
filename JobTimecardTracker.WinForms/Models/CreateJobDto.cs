
namespace JobTimecardTracker.WinForms.Models;

public class CreateJobDto
{
    public string JobNumber { get; set; } = string.Empty;
    public JobType JobType { get; set; }
    public DateTime JobDate { get; set; } = DateTime.Today;
    public string CompanyName { get; set; } = string.Empty;
    public string ContactName { get; set; } = string.Empty;
    public string TelephoneNumber { get; set; } = string.Empty;
}
