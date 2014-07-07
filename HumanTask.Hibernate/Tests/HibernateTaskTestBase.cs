using NHibernate;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Mapping.ByCode;
using NHibernate.Tool.hbm2ddl;

namespace HumanTask.Hibernate.Tests
{
    public class HibernateTaskTestBase
    {
        protected static ISessionFactory SessionFactory;
        protected static SchemaExport SchemaExport;

        protected void OnFixtureSetUp()
        {
            Configuration c = new Configuration().Proxy(
                p => p.ProxyFactoryFactory<NHibernate.Bytecode.DefaultProxyFactoryFactory>())
                .DataBaseIntegration(d =>
                {
                    d.Dialect<MsSql2008Dialect>();
                    d.ConnectionString = "Server=localhost;Initial Catalog=Tasks;user=jbpmUser;password=7bpmXus3r";
                    d.LogSqlInConsole = true;

                });
            c.SetProperty("current_session_context_class", "thread_static");
            ModelMapper mapper = new ModelMapper();
            mapper.AddMapping(typeof(TaskEntityMap));
            c.AddMapping(mapper.CompileMappingForAllExplicitlyAddedEntities());
            SchemaExport = new SchemaExport(c);
            SessionFactory = c.BuildSessionFactory();
        }

        protected void OnSetUp()
        {
            SchemaExport.Drop(true, true);
            SchemaExport.Execute(true, true, false);
        }

        protected void OnTestTearDown()
        {
            SchemaExport.Drop(true, true);
        }

    }
}
