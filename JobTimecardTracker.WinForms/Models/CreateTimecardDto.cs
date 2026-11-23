namespace JobTimecardTracker.WinForms.Models;

public class CreateTimecardDto
{
    public string Employee { get; set; } = string.Empty;
    public DateTime DateWorked { get; set; } = DateTime.Today;
    public decimal HoursWorked { get; set; }
    public Guid JobId { get; set; }
}
