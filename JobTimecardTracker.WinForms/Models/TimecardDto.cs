
namespace JobTimecardTracker.WinForms.Models;

public class TimecardDto
{
    public Guid Id { get; set; }
    public string Employee { get; set; } = string.Empty;
    public DateTime DateWorked { get; set; }
    public decimal HoursWorked { get; set; }
    public Guid JobId { get; set; }
    public string JobNumber { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
}
