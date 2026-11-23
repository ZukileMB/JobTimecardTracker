using FluentNHibernate.Mapping;
using JobTimecardTracker.Domain;

namespace JobTimecardTracker.Data;

public class JobMap : ClassMap<Job>
{
    public JobMap()
    {
        Table("Jobs");
        Id(x => x.Id).GeneratedBy.GuidComb();
        Map(x => x.JobNumber).Not.Nullable().Unique();
        Map(x => x.JobType).CustomType<int>().Not.Nullable();
        Map(x => x.JobDate).CustomSqlType("date").Not.Nullable();
        Map(x => x.CompanyName).Not.Nullable();
        Map(x => x.ContactName).Not.Nullable();
        Map(x => x.TelephoneNumber).Not.Nullable();
        HasMany(x => x.Timecards)
            .KeyColumn("JobId")
            .Inverse()
            .Cascade.AllDeleteOrphan();
    }
}

public class TimecardMap : ClassMap<Timecard>
{
    public TimecardMap()
    {
        Table("Timecards");
        Id(x => x.Id).GeneratedBy.GuidComb();
        Map(x => x.Employee).Not.Nullable();
        Map(x => x.DateWorked).CustomSqlType("date").Not.Nullable();
        Map(x => x.HoursWorked).Not.Nullable();
        References(x => x.Job)
            .Column("JobId")
            .Not.Nullable();
    }
}