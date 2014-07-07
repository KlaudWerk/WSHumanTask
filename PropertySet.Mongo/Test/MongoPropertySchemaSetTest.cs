using Klaudwerk.PropertySet.Test;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using NUnit.Framework;

namespace Klaudwerk.PropertySet.Mongo.Test
{
    [TestFixture]
    public class MongoPropertySchemaSetTest:PropertySchemaSetTest
    {
        public const string PropSetCollectionsDb = "ut_propertyset_db";
        public const string TestCollectionName = "ut_dictionary";
        public const string ConnStr = "mongodb://Atlanta";
        private MongoServer _server;
        private MongoDatabase _testDb;
        private MongoCollection<BsonDocument> _collection;

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
            if (_testDb.CollectionExists(TestCollectionName))
                _testDb.DropCollection(TestCollectionName);
            _collection = _testDb.GetCollection<BsonDocument>(TestCollectionName);
            _collection.EnsureIndex(IndexKeys.Ascending("key"), IndexOptions.SetUnique(true));

        }
        [TearDown]
        public void TestTearDown()
        {
            if (_collection != null)
            {
                if (_testDb.CollectionExists(TestCollectionName))
                    _testDb.DropCollection(TestCollectionName);
            }
        }

        protected override IPropertySchemaSet GetSchemaSet()
        {
            return new MongoPropertySchemaSet(_collection, new PropertySchemaFactory());
        }
    }
}
