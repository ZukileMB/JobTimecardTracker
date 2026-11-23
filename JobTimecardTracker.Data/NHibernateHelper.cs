using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Driver;
using NHibernate.Tool.hbm2ddl;

namespace JobTimecardTracker.Data;

public static class NHibernateHelper
{
    public static ISessionFactory CreateSessionFactory(string connectionString)
    {
        return Fluently.Configure()
            .Database(MsSqlConfiguration.MsSql2012
                .Driver<MicrosoftDataSqlClientDriver>()
                .ConnectionString(connectionString)
                .ShowSql())
            .Mappings(m => m.FluentMappings
                .AddFromAssemblyOf<JobMap>())
            .ExposeConfiguration(cfg => new SchemaUpdate(cfg).Execute(false, true))
            .BuildSessionFactory();
    }
}