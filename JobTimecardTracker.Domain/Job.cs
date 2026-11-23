namespace JobTimecardTracker.Domain;

public class Job
{
    public virtual Guid Id { get; set; } = Guid.NewGuid();
    public virtual string JobNumber { get; set; } = string.Empty;
    public virtual JobType JobType { get; set; }
    public virtual DateTime JobDate { get; set; }
    public virtual string CompanyName { get; set; } = string.Empty;
    public virtual string ContactName { get; set; } = string.Empty;
    public virtual string TelephoneNumber { get; set; } = string.Empty;
    public virtual IList<Timecard> Timecards { get; set; } = new List<Timecard>();
}