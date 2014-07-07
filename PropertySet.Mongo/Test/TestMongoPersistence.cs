using Klaudwerk.PropertySet.Test;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using NUnit.Framework;

namespace Klaudwerk.PropertySet.Mongo.Test
{
    [TestFixture]
    public class TestMongoPersistence:ValueSetCollectionTest
    {
        public const string PropSetCollectionsDb = "ut_db";
        public const string ConnStr = "mongodb://Atlanta";
        public const string TestCollectionId = "E6F1B8E0-E701-42D0-B49F-C51792560A6D";
        private MongoServer _server;
        private MongoDatabase _testDb;
        private MongoPropertySetCollection _collection;

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
            _collection=new MongoPropertySetCollection(_testDb,"properties",new PropertySchemaFactory());

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
            MongoServer server = MongoServer.Create(ConnStr);
            if (server.DatabaseExists(PropSetCollectionsDb))
                server.DropDatabase(PropSetCollectionsDb);
            MongoDatabase testDb = server.GetDatabase(PropSetCollectionsDb);
            if (testDb.CollectionExists("ut_collection"))
                testDb.DropCollection("ut_collection");
            
            MongoCollection<BsonDocument> collection =
                testDb.GetCollection<BsonDocument>("ut_collection");
            collection.EnsureIndex(IndexKeys.Ascending("key"),IndexOptions.SetUnique(true));
         
            BsonDocument el = new BsonDocument("key","one");
            collection.Insert(el,SafeMode.True);
            collection.Insert(new BsonDocument("key", "two"), SafeMode.True);
            try
            {
                collection.Insert(new BsonDocument("key", "one"), SafeMode.True);
                Assert.Fail("Duplicate key insert");
            }
            catch(MongoSafeModeException ex)
            {
                
            }
            Assert.AreEqual(2, collection.Count());
            QueryDocument query=new QueryDocument("key","one");
            BsonDocument ee = collection.FindOne(query);
            Assert.IsNotNull(ee);
        }

        /// <summary>
        /// Instantiates Mongo collection
        /// </summary>
        /// <returns></returns>
        protected override IPropertySetCollection GetCollection()
        {
            return _collection;
        }
    }
}
