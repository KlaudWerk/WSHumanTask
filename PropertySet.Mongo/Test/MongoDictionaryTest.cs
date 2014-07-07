using System;
using System.Collections;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using NUnit.Framework;

namespace Klaudwerk.PropertySet.Mongo.Test
{
    [TestFixture]
    public class MongoDictionaryTest
    {
        public const string PropSetCollectionsDb = "ut_propertyset_db";
        public const string TestCollectionName = "ut_dictionary";
        public const string ConnStr = "mongodb://Atlanta";
        private MongoServer _server;
        private MongoDatabase _testDb;
        private MongoCollection<BsonDocument> _collection;
        private IDictionary<string, string> _testDictionary;

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
            _collection =_testDb.GetCollection<BsonDocument>(TestCollectionName);
            _collection.EnsureIndex(IndexKeys.Ascending("key"),IndexOptions.SetUnique(true));
            _testDictionary = new TestMongoCollectionDictionary(_collection);

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

        [Test]
        public void TestGetEnumerator()
        {
            _testDictionary.Add("key-1", "value-1");
            _testDictionary.Add("key-2", "value-2");
            _testDictionary.Add("key-3", "value-3");
            _testDictionary.Add("key-4", "value-4");
            Assert.AreEqual(4, _collection.Count());
            IEnumerator<KeyValuePair<string,string>> en= _testDictionary.GetEnumerator();
            Assert.IsNotNull(en);
            while(en.MoveNext())
            {
                Assert.IsNotNull(en.Current);
                switch (en.Current.Key)
                {
                    case "key-1":
                        Assert.AreEqual(en.Current.Value, "value-1");
                        break;
                    case "key-2":
                        Assert.AreEqual(en.Current.Value, "value-2");
                        break;
                    case "key-3":
                        Assert.AreEqual(en.Current.Value, "value-3");
                        break;
                    case "key-4":
                        Assert.AreEqual(en.Current.Value, "value-4");
                        break;
                }
            }
        }

        [Test]
        public void  TestIEnumerableGetEnumerator()
        {
            _testDictionary.Add("key-1", "value-1");
            _testDictionary.Add("key-2", "value-2");
            _testDictionary.Add("key-3", "value-3");
            _testDictionary.Add("key-4", "value-4");
            Assert.AreEqual(4, _collection.Count());
            IEnumerator en= ((IEnumerable) _testDictionary).GetEnumerator();
            Assert.IsNotNull(en);
            while (en.MoveNext())
            {
                Assert.IsNotNull(en.Current);
                KeyValuePair<string, string> kvp = (KeyValuePair<string, string>) en.Current;
                switch (kvp.Key)
                {
                    case "key-1":
                        Assert.AreEqual(kvp.Value, "value-1");
                        break;
                    case "key-2":
                        Assert.AreEqual(kvp.Value, "value-2");
                        break;
                    case "key-3":
                        Assert.AreEqual(kvp.Value, "value-3");
                        break;
                    case "key-4":
                        Assert.AreEqual(kvp.Value, "value-4");
                        break;
                }

            }

        }

        [Test]
        public void TestAddKeyValuePair()
        {
            Assert.AreEqual(0, _collection.Count());
            KeyValuePair<string, string> kvp = new KeyValuePair<string, string>("key-2", "value-2");
            _testDictionary.Add(kvp);
            Assert.AreEqual(1, _collection.Count());
            QueryDocument q = new QueryDocument(new Dictionary<string, object> { { "key", "key-2" } });
            Assert.AreEqual(1, _collection.Find(q).Count());

        }
        
        [Test]
        public void TestClearCollection()
        {
            _testDictionary.Add("key-1", "value-1");
            _testDictionary.Add("key-2", "value-2");
            _testDictionary.Add("key-3", "value-3");
            _testDictionary.Add("key-4", "value-4");
            Assert.AreEqual(4, _collection.Count());
            _testDictionary.Clear();
            Assert.AreEqual(0, _collection.Count());
        }

        [Test]
        public void TestContainsKeyValuePair()
        {
            _testDictionary.Add("key-1", "value-1");
            _testDictionary.Add("key-2", "value-2");
            _testDictionary.Add("key-3", "value-3");
            _testDictionary.Add("key-4", "value-4");
            Assert.AreEqual(4, _collection.Count());
            KeyValuePair<string, string> kvp = new KeyValuePair<string, string>("key-2", "value-2");
            Assert.IsTrue(_testDictionary.Contains(kvp));
        }
        
        [Test]
        public void TestCopyToKeyValuePair()
        {
            _testDictionary.Add("key-1", "value-1");
            _testDictionary.Add("key-2", "value-2");
            _testDictionary.Add("key-3", "value-3");
            _testDictionary.Add("key-4", "value-4");
            Assert.AreEqual(4, _collection.Count());
            KeyValuePair<string,string>[] kvp=new KeyValuePair<string, string>[4];
            _testDictionary.CopyTo(kvp,0);
            foreach (KeyValuePair<string, string> keyValuePair in kvp)
            {
                Assert.IsNotNull(keyValuePair);
                switch (keyValuePair.Key)
                {
                    case "key-1":
                        Assert.AreEqual(keyValuePair.Value,"value-1");
                        break;
                    case "key-2":
                        Assert.AreEqual(keyValuePair.Value, "value-2");
                        break;
                    case "key-3":
                        Assert.AreEqual(keyValuePair.Value, "value-3");
                        break;
                    case "key-4":
                        Assert.AreEqual(keyValuePair.Value, "value-4");
                        break;

                }
            }
            
        }

        [Test]
        public void TestRemoveKeyValuePair()
        {
            _testDictionary.Add("key-1", "value-1");
            _testDictionary.Add("key-2", "value-2");
            _testDictionary.Add("key-3", "value-3");
            _testDictionary.Add("key-4", "value-4");
            Assert.AreEqual(4, _collection.Count());
            KeyValuePair<string, string> kvp = new KeyValuePair<string, string>("key-2", "value-2");
            Assert.IsTrue(_testDictionary.Remove(kvp));
            Assert.AreEqual(3, _collection.Count());
            QueryDocument q=new QueryDocument(new Dictionary<string, object>{{"key","key-2"}});
            Assert.AreEqual(0, _collection.Find(q).Count());
        }

        [Test]
        public void TestGetCount()
        {
            _testDictionary.Add("key-1", "value-1");
            _testDictionary.Add("key-2", "value-2");
            _testDictionary.Add("key-3", "value-3");
            _testDictionary.Add("key-4", "value-4");
            Assert.AreEqual(4, _collection.Count());
            Assert.AreEqual(4,_testDictionary.Count);
            
        }
        [Test]
        public void TestIsReadOnly()
        {
            Assert.IsFalse(_testDictionary.IsReadOnly);   
        }

        [Test]
        public void TestContainsKey()
        {
            _testDictionary.Add("key-1", "value-1");
            _testDictionary.Add("key-2", "value-2");
            _testDictionary.Add("key-3", "value-3");
            _testDictionary.Add("key-4", "value-4");
            Assert.AreEqual(4, _collection.Count());
            Assert.IsTrue(_testDictionary.ContainsKey("key-1"));
            Assert.IsFalse(_testDictionary.ContainsKey("key-33"));
        }

        [Test]
        public void TestAddKeyValue()
        {
            _testDictionary.Add("key-1", "value-1");
            _testDictionary.Add("key-2", "value-2");
            _testDictionary.Add("key-3", "value-3");
            _testDictionary.Add("key-4", "value-4");
            Assert.AreEqual(4,_collection.Count());
            QueryDocument q=new QueryDocument("key","key-1");
            Assert.AreEqual(1, _collection.Find(q).Count());
            q = new QueryDocument("key", "key-2");
            Assert.AreEqual(1, _collection.Find(q).Count());
            q = new QueryDocument("key", "key-3");
            Assert.AreEqual(1, _collection.Find(q).Count());
            q = new QueryDocument("key", "key-4");
            Assert.AreEqual(1, _collection.Find(q).Count());
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestAddKeyValueDuplicateKeyFail()
        {
            _testDictionary.Add("key-1", "value-1");
            Assert.AreEqual(1, _collection.Count());
            _testDictionary.Add("key-1", "value-2");
        }

        [Test]
        public void TestRemoveKey()
        {
            _testDictionary.Add("key-1", "value-1");
            _testDictionary.Add("key-2", "value-2");
            Assert.AreEqual(2, _collection.Count());
            Assert.IsTrue(_testDictionary.Remove("key-1"));
            Assert.AreEqual(1, _collection.Count());
        }

        [Test]
        public void TestRemoveKeyNotExist()
        {
            _testDictionary.Add("key-1", "value-1");
            _testDictionary.Add("key-2", "value-2");
            Assert.AreEqual(2, _collection.Count());
            Assert.IsFalse(_testDictionary.Remove("key-3"));
            Assert.AreEqual(2, _collection.Count());
        }

        [Test]
        public void TestTryGetValue()
        {
            _testDictionary.Add("key-1", "value-1");
            _testDictionary.Add("key-2", "value-2");
            Assert.AreEqual(2, _collection.Count());
            string val;
            Assert.IsTrue(_testDictionary.TryGetValue("key-1",out val));
            Assert.AreEqual("value-1",val);
            Assert.IsFalse(_testDictionary.TryGetValue("key-3", out val));
        }

        [Test]
        public void TestIndexerGet()
        {
            _testDictionary.Add("key-1", "value-1");
            _testDictionary.Add("key-2", "value-2");
            Assert.AreEqual(2, _collection.Count());
            string val = _testDictionary["key-1"];
            Assert.AreEqual("value-1",val);
            val = _testDictionary["key-2"];
            Assert.AreEqual("value-2", val);
            val = _testDictionary["key-3"];
            Assert.IsNull(val);
        }

        [Test]
        public void TestIndexerSet()
        {
            _testDictionary["key-1"] = "value-1";
            _testDictionary["key-2"] = "value-2";
            Assert.AreEqual(2, _collection.Count());
            QueryDocument q = new QueryDocument("key", "key-1");
            Assert.AreEqual(1, _collection.Find(q).Count());
            BsonValue val= _collection.FindOne(q)["value"];
            Assert.IsNotNull(val);
            Assert.AreEqual("value-1", val.AsString);   
            q = new QueryDocument("key", "key-2");
            Assert.AreEqual(1, _collection.Find(q).Count());
            val = _collection.FindOne(q)["value"];
            Assert.IsNotNull(val);
            Assert.AreEqual("value-2", val.AsString);   
            _testDictionary["key-2"] = "value-3";
            q = new QueryDocument("key", "key-2");
            Assert.AreEqual(1, _collection.Find(q).Count());
            val = _collection.FindOne(q)["value"];
            Assert.IsNotNull(val);
            Assert.AreEqual("value-3", val.AsString);
        }

        [Test]
        public void TestGetKeysCollection()
        {
            _testDictionary.Add("key-1", "value-1");
            _testDictionary.Add("key-2", "value-2");
            _testDictionary.Add("key-3", "value-2");
            _testDictionary.Add("key-4", "value-2");
            Assert.AreEqual(4,_collection.Count());
            ICollection<string> keys = _testDictionary.Keys;
            Assert.AreEqual(4,keys.Count);
            for(int i=1; i<=4; i++)
            {
                Assert.IsTrue(keys.Contains("key-" + i));
            }
        }

        [Test]
        public void TestGetValuesCollection()
        {
            _testDictionary.Add("key-1", "value-1");
            _testDictionary.Add("key-2", "value-2");
            _testDictionary.Add("key-3", "value-3");
            _testDictionary.Add("key-4", "value-4");
            Assert.AreEqual(4, _collection.Count());
            ICollection<string> values = _testDictionary.Values;
            Assert.AreEqual(4, values.Count);
            for (int i = 1; i <= 4; i++)
            {
                Assert.IsTrue(values.Contains("value-" + i));
            }
            
        }
     
        private class TestMongoCollectionDictionary:MongoCollectionDictionary<string,string>
        {
            #region Overrides of MongoCollectionDictionary<BsonDocument,string,string>

            public TestMongoCollectionDictionary(MongoCollection<BsonDocument> collection) : base(collection)
            {
            }

            protected override BsonDocument ConvertToCollectionElementValue(string key, string value)
            {
                return new BsonDocument(new Dictionary<string, object> {
                 {"key",key},   
                {"value", value},
                {"_id",new ObjectId()}});
            }

            protected override BsonValue ToBsonValue(string key)
            {
                return key;
            }

            /// <summary>
            /// Gets the name of the key field.
            /// </summary>
            /// <returns></returns>
            protected override string GetKeyField()
            {
                return "key";
            }

            /// <summary>
            /// Gets the key field value.
            /// </summary>
            /// <param name="value">The value.</param>
            /// <returns></returns>
            protected override string GetKeyFieldValue(BsonDocument value)
            {
                return value["key"].AsString;
            }
            /// <summary>
            /// Gets the updates.
            /// </summary>
            /// <param name="key">The key.</param>
            /// <param name="value">The value.</param>
            /// <returns></returns>
            protected override UpdateBuilder GetUpdates(string key,string value)
            {
                return Update.Set("value", value);
            }

            protected override string ConvertValue(BsonDocument mongoElement)
            {
                return mongoElement==null?null: mongoElement["value"].AsString;
            }

            protected override bool TryConvertValue(BsonDocument mongoElement, out string value)
            {
                value = ConvertValue(mongoElement);
                return value != null;
            }

            #endregion
        }
    }
}
