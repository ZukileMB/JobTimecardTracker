namespace JobTimecardTracker.Domain;

public class Timecard
{
    public virtual Guid Id { get; set; } = Guid.NewGuid();
    public virtual string Employee { get; set; } = string.Empty;
    public virtual DateTime DateWorked { get; set; }
    public virtual decimal HoursWorked { get; set; }
    public virtual Guid JobId { get; set; }
    public virtual Job? Job { get; set; }
}