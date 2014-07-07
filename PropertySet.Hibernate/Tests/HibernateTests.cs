using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Klaudwerk.PropertySet.Serialization;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Mapping.ByCode;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;

namespace Klaudwerk.PropertySet.Hibernate.Tests
{
    [TestFixture]
    public class HibernateTests
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
            ModelMapper mapper=new ModelMapper();
            mapper.AddMapping(typeof(PropertySetCollectionMap));
            c.AddMapping(mapper.CompileMappingForAllExplicitlyAddedEntities());
            _schemaExport= new SchemaExport(c);
            _sessionFactory = c.BuildSessionFactory();
        }
        [SetUp]
        public void SetUp()
        {
            _schemaExport.Drop(true, true);
            _schemaExport.Execute(true, true, false);
        }
        [TearDown]
        public void TearDown()
        {
            _schemaExport.Drop(true, true);
        }
        [Test]
        public void TestCollectionStore()
        {
            ISession session = _sessionFactory.OpenSession();
            var h = new PropertyElementsCollection()
            {
                Id = Guid.NewGuid(),
                Elements = new Dictionary<string, HibernatePropertyElement>()
            };
            h.Elements["name1"] = new HibernatePropertyElement { SerializationHint = SerializationTypeHint.Int };
            h.Elements["name2"] = new HibernatePropertyElement { SerializationHint = SerializationTypeHint.Int };
            session.Save(h);
            session.Flush();
            
        }
        [Test]
        public void Test()
        {
            HibernatePropertySetCollection collection=new HibernatePropertySetCollection(new PropertySchemaFactory(), _sessionFactory,Guid.NewGuid());
            collection["name"] = "Value";
            collection["number"] = 345;
            collection["date"] = DateTime.UtcNow;
            collection["bool"] = true;
            collection["long"] = Int64.MaxValue;
            collection["double"] = 0.05d;
            byte[] b=new byte[1024];
            for (int i = 0; i < b.Length; i++)
                b[i] = (byte)i;
            collection["raw"] = b;

            object v = collection["name"];
            Assert.IsNotNull(v);
            Assert.AreEqual("Value",v);
        }
    }
}
