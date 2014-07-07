using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HumanTask.ValueSet;
using HumanTask.ValueSet.Persistence.Mongo;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using NUnit.Framework;

namespace HumanTask.Test
{
    [TestFixture]
    public class TestMongoPersistence:ValueSetCollectionTest
    {
        public const string PropSetCollectionsDb = "ut_propertyset_db";
        public const string ConnStr = "mongodb://localhost";
        public const string TestCollectionId = "E6F1B8E0-E701-42D0-B49F-C51792560A6D";
        private MongoServer _server;
        private MongoDatabase _testDb;
        private MongoValueSetCollection _collection;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            _server = MongoServer.Create(ConnStr);
            if (_server.DatabaseExists(PropSetCollectionsDb))
                _server.DropDatabase(PropSetCollectionsDb);
            _testDb = _server.GetDatabase(PropSetCollectionsDb);
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            if (_server.DatabaseExists(PropSetCollectionsDb))
                _server.DropDatabase(PropSetCollectionsDb);
        }

        [SetUp]
        public void TestSetUp()
        {
            string collectionName = string.Format("vs-{0}", TestCollectionId);
            if (_testDb.CollectionExists(collectionName))
                _testDb.DropCollection(collectionName);
            _collection=new MongoValueSetCollection(_testDb,new Guid(TestCollectionId),new PropertySchemaFactory());

        }
        [TearDown]
        public void TestTearDown()
        {
            if (_collection != null)
            {
                if (_testDb.CollectionExists(_collection.CollectionName))
                    _testDb.DropCollection(_collection.CollectionName);
            }
        }
        [Test]
        public void TestMongoInteraction()
        {
            MongoServer server = MongoServer.Create();
            if (server.DatabaseExists(PropSetCollectionsDb))
                server.DropDatabase(PropSetCollectionsDb);
            MongoDatabase testDb = server.GetDatabase(PropSetCollectionsDb);
            if (testDb.CollectionExists("ut_collection"))
                testDb.DropCollection("ut_collection");
            
            MongoCollection<MongoPropertyElement> collection =
                testDb.GetCollection<MongoPropertyElement>("ut_collection");
            collection.EnsureIndex(IndexKeys.Ascending("Name"),IndexOptions.SetUnique(true));
         
            MongoPropertyElement el = new MongoPropertyElement("one");
            collection.Insert(el,SafeMode.True);
            collection.Insert(new MongoPropertyElement("two"),SafeMode.True);
            try
            {
                collection.Insert(new MongoPropertyElement("one"), SafeMode.True);
                Assert.Fail("Duplicate key insert");
            }
            catch(MongoSafeModeException ex)
            {
                
            }
            Assert.AreEqual(2, collection.Count());
            QueryDocument query=new QueryDocument("Name","one");
            MongoPropertyElement ee = collection.FindOne(query);
            Assert.IsNotNull(ee);
        }

        /// <summary>
        /// Instantiates Mongo collection
        /// </summary>
        /// <returns></returns>
        protected override IValueSetCollection GetCollection()
        {
            return _collection;
        }
    }
}
