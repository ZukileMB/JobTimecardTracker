using JobTimecardTracker.Domain;
using Xunit;

namespace JobTimecardTracker.Tests;

public class TimecardRulesTests
{
    [Fact]
    public void TotalHoursForEmployeeOnSameDay_SumsMultipleJobs()
    {
        var workDate = new DateTime(2026, 11, 23);

        var jobA = new Job
        {
            Id = Guid.NewGuid(),
            JobNumber = "JOB-101",
            CompanyName = "Acme Corp",
            ContactName = "Jane Doe",
            TelephoneNumber = "0215550101"
        };

        var jobB = new Job
        {
            Id = Guid.NewGuid(),
            JobNumber = "JOB-102",
            CompanyName = "Beta Logistics",
            ContactName = "John Smith",
            TelephoneNumber = "0215550102"
        };

        var entries = new List<Timecard>
        {
            new() { Id = Guid.NewGuid(), Employee = "CAT VIP", DateWorked = workDate, HoursWorked = 4.5m, JobId = jobA.Id, Job = jobA },
            new() { Id = Guid.NewGuid(), Employee = "CAT VIP", DateWorked = workDate, HoursWorked = 3.25m, JobId = jobB.Id, Job = jobB },
            new() { Id = Guid.NewGuid(), Employee = "ABC", DateWorked = workDate.AddDays(1), HoursWorked = 8.0m, JobId = jobA.Id, Job = jobA },
            new() { Id = Guid.NewGuid(), Employee = "Other Tech", DateWorked = workDate, HoursWorked = 6.0m, JobId = jobA.Id, Job = jobA }
        };

        decimal totalHours = entries
            .Where(t => string.Equals(t.Employee, "CAT VIP", StringComparison.OrdinalIgnoreCase)
                      && t.DateWorked.Date == workDate.Date)
            .Sum(t => t.HoursWorked);

        Assert.Equal(7.75m, totalHours);
    }
}