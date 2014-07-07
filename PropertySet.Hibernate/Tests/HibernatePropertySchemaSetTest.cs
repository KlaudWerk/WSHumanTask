using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Klaudwerk.PropertySet.Test;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Mapping.ByCode;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;

namespace Klaudwerk.PropertySet.Hibernate.Tests
{
    [TestFixture]
    public class HibernatePropertySchemaSetTest : PropertySchemaSetTest
    {
        private static ISessionFactory _sessionFactory;
        private static SchemaExport _schemaExport;
        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            Configuration c = new Configuration().Proxy(
                p => p.ProxyFactoryFactory<NHibernate.Bytecode.DefaultProxyFactoryFactory>())
                .DataBaseIntegration(d =>
                {
                    d.Dialect<MsSql2008Dialect>();
                    d.ConnectionString = "Server=atlanta;Initial Catalog=PropertySet;user=jbpmUser;password=7bpmXus3r";
                    d.LogSqlInConsole = true;

                });
            c.SetProperty("current_session_context_class", "thread_static");
            ModelMapper mapper = new ModelMapper();
            mapper.AddMapping(typeof(PropertySetCollectionMap));
            c.AddMapping(mapper.CompileMappingForAllExplicitlyAddedEntities());
            _schemaExport = new SchemaExport(c);
            _sessionFactory = c.BuildSessionFactory();
        }
        [SetUp]
        public void SetUp()
        {
            _schemaExport.Drop(true, true);
            _schemaExport.Execute(true, true, false);
        }
        [TearDown]
        public void TestTearDown()
        {
            _schemaExport.Drop(true, true);
        }

        protected override IPropertySchemaSet GetSchemaSet()
        {
            return new HibernatePropertySetCollection(new PropertySchemaFactory(), _sessionFactory, Guid.NewGuid()).Schemas;
        }
    }
}
