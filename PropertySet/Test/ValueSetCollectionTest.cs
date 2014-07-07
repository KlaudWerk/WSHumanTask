using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;

namespace Klaudwerk.PropertySet.Test
{
    [Serializable]
    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
    [TestFixture]
    public class ValueSetCollectionTest
    {
        private readonly Dictionary<string, IValueSchema<object>> _schemaStorage = new Dictionary<string, IValueSchema<object>>();

        [TearDown]
        public void TearDown()
        {
            _schemaStorage.Clear();
        }

        #region Class (string) tests
        [Test]
        public void TestAddStringPropertyWithDefaultValue()
        {
            IPropertySetCollection collection = GetCollection();
            collection.Add("key", new StringSchema {DefaultValue = "default"});
            string value = collection.Get<string>("key");
            Assert.IsNotNull(value);
            Assert.AreEqual("default",value);
            IValueSchema<object> s =collection.Schemas.GetSchema("key");
            Assert.IsNotNull(s);
            Assert.AreEqual(typeof(string),s.Type);
        }

        [Test]
        public void TestAddStringPropertyNoDefaultValueAllowNull()
        {
            IPropertySetCollection collection = GetCollection();
            collection.Add("key", new StringSchema{AllowNull = true});
            string value = collection.Get<string>("key");
            Assert.IsNull(value);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestAddStringPropertyNoDefaultValueNotAllowNull()
        {
            IPropertySetCollection collection = GetCollection();
            collection.Add("key", new StringSchema{AllowNull = false});
        }

        [Test]
        public void TestAddStringPropertyNoDefaultValueNotAllowNullWithValue()
        {
            IPropertySetCollection collection = GetCollection();
            collection.Add("key","value", new StringSchema());
            string value = collection.Get<string>("key");
            Assert.IsNotNull(value);
            Assert.AreEqual("value", value);
        }

        [Test]
        public void TestStringSetValue()
        {
            IPropertySetCollection collection = GetCollection();
            collection.Add("key", new StringSchema {AllowNull = true});
            string value = collection.Get<string>("key");
            Assert.IsNull(value);
            collection.Set("key","value");
            value = collection.Get<string>("key");
            Assert.IsNotNull(value);
            Assert.AreEqual("value",value);
        }

        [Test]
        public void TestStringGetValueViaDictionary()
        {
            IPropertySetCollection collection = GetCollection();
            collection.Add("key", new StringSchema { AllowNull = true });
            collection.Set("key", "value");
            object value = collection["key"];
            Assert.IsNotNull(value);
            Assert.AreEqual("value", value);
        }
        [Test]
        public void TestStringSetValueViaDictionary()
        {
            IPropertySetCollection collection = GetCollection();
            collection.Add("key", new StringSchema { AllowNull = true });
            string value = collection.Get<string>("key");
            Assert.IsNull(value);
            collection["key"]="value";
            value = collection.Get<string>("key");
            Assert.IsNotNull(value);
            Assert.AreEqual("value", value);
        }


        [Test]
        [ExpectedException(typeof(PropertyValidationException))]
        public void TestAddStringOutsideOfTheirAllowedValues()
        {
            IPropertySetCollection collection = GetCollection();
            collection.Add("key", new StringSchema {AllowNull = true, PossibleValues = new[] {"one", "two", "tree"}});
            collection.Set("key","1");
        }

        [Test]
        [ExpectedException(typeof(PropertyValidationException))]
        public void TestAddStringOutsideOfTheiirAllowedValuesViaDictionary()
        {
            IPropertySetCollection collection = GetCollection();
            collection.Add("key", new StringSchema { AllowNull = true, PossibleValues = new[] { "one", "two", "tree" } });
            collection["key"]="1";
        }

        [Test]
        [ExpectedException(typeof(PropertyValidationException))]
        public void TestStringSetInvalidType()
        {
            IPropertySetCollection collection = GetCollection();
            collection.Add("key", new StringSchema { AllowNull = true });
            string value = collection.Get<string>("key");
            Assert.IsNull(value);
            collection["key"] = "value";
            value = collection.Get<string>("key");
            Assert.IsNotNull(value);
            Assert.AreEqual("value", value);
            collection.Set("key",(int?)200);
        }

        [Test]
        [ExpectedException(typeof(PropertyValidationException))]
        public void TestStringSetInvalidTypeViaDictionary()
        {
            IPropertySetCollection collection = GetCollection();
            collection.Add("key", new StringSchema { AllowNull = true });
            string value = collection.Get<string>("key");
            Assert.IsNull(value);
            collection["key"] = "value";
            value = collection.Get<string>("key");
            Assert.IsNotNull(value);
            Assert.AreEqual("value", value);
            collection["key"] =200;
        }

        [Test]
        public void TestStringSchemaValidationSuccess()
        {
            IPropertySetCollection collection = GetCollection();
            collection.Add("key", new StringSchema { AllowNull = true,MaxLength = 10,MinLength = 5});
            collection.Set("key", "123456");
            string value = collection.Get<string>("key");
            Assert.IsNotNull(value);
            Assert.AreEqual("123456",value);

        }

        [Test]
        [ExpectedException(typeof(PropertyValidationException))]
        public void TestStringSchemaValidationFailMinValue()
        {
            IPropertySetCollection collection = GetCollection();
            collection.Add("key", new StringSchema { AllowNull = true, MaxLength = 10, MinLength = 5 });
            collection.Set("key", "123");
        }

        [Test]
        [ExpectedException(typeof(PropertyValidationException))]
        public void TestStringSchemaValidationFailMaxValue()
        {
            IPropertySetCollection collection = GetCollection();
            collection.Add("key", new StringSchema { AllowNull = true, MaxLength = 10, MinLength = 5 });
            collection.Set("key", "123456789012345");

        }
        #endregion

        #region Value Type tests
        [Test]
        public void TestAddValueTypePropertyWithDefaultValue()
        {
            IPropertySetCollection collection = GetCollection();
            collection.Add("key", new IntSchema { DefaultValue = 5 });
            int? value = collection.Get<int?>("key");
            Assert.IsNotNull(value);
            Assert.AreEqual(5, value);
            IValueSchema<object> s = collection.Schemas.GetSchema("key");
            Assert.IsNotNull(s);
            Assert.AreEqual(typeof(int?), s.Type);
        }

        [Test]
        public void TestAddValueTypePropertyNoDefaultValueAllowNull()
        {
            IPropertySetCollection collection = GetCollection();
            collection.Add("key", new IntSchema { AllowNull = true });
            int? value = collection.Get<int?>("key");
            Assert.IsFalse(value.HasValue);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestAddValueTypePropertyNoDefaultValueNotAllowNull()
        {
            IPropertySetCollection collection = GetCollection();
            collection.Add("key", new IntSchema{AllowNull = false});
        }

        [Test]
        public void TestAddValueTypePropertyNoDefaultValueNotAllowNullWithValue()
        {
            IPropertySetCollection collection = GetCollection();
            collection.Add("key", 100, new IntSchema());
            int? value = collection.Get<int?>("key");
            Assert.IsNotNull(value);
            Assert.AreEqual(100, value);
        }

        [Test]
        public void TestValueTypeSetValue()
        {
            IPropertySetCollection collection = GetCollection();
            collection.Add("key", new IntSchema { AllowNull = true });
            int? value = collection.Get<int?>("key");
            Assert.IsFalse(value.HasValue);
            collection.Set("key", (int?)100);
            value = collection.Get<int>("key");
            Assert.IsNotNull(value);
            Assert.AreEqual(100, value);
        }

        [Test]
        public void TestSValueTypeGetValueViaDictionary()
        {
            IPropertySetCollection collection = GetCollection();
            collection.Add("key", new IntSchema{ AllowNull = true });
            collection.Set("key", (int?)100);
            object value = collection["key"];
            Assert.IsNotNull(value);
            Assert.AreEqual(100, value);
        }
        [Test]
        public void TestValueTypeSetValueViaDictionary()
        {
            IPropertySetCollection collection = GetCollection();
            collection.Add("key", new IntSchema { AllowNull = true });
            int? value = collection.Get<int?>("key");
            Assert.IsFalse(value.HasValue);
            collection["key"] =100;
            value = collection.Get<int>("key");
            Assert.IsNotNull(value);
            Assert.AreEqual(100, value);
        }


        [Test]
        [ExpectedException(typeof(PropertyValidationException))]
        public void TestAddValueTypeOutsideOfTheSetOfAllowedValues()
        {
            IPropertySetCollection collection = GetCollection();
            collection.Add("key", new IntSchema { AllowNull = true, PossibleValues = new int?[] { 2, 3, 4 } });
            collection.Set("key", (int?)1);
        }

        [Test]
        [ExpectedException(typeof(PropertyValidationException))]
        public void TestAddValueTypeOutsideOfTheSetOfAllowedValuesViaDictionary()
        {
            IPropertySetCollection collection = GetCollection();
            collection.Add("key", new IntSchema { AllowNull = true, PossibleValues = new int?[] { 2, 3, 4 } });
            collection["key"]=1;
        }

        [Test]
        [ExpectedException(typeof(PropertyValidationException))]
        public void TestValueTypeSetInvalidType()
        {
            IPropertySetCollection collection = GetCollection();
            collection.Add("key", new IntSchema { AllowNull = true });
            int? value = collection.Get<int?>("key");
            Assert.IsFalse(value.HasValue);
            collection["key"] = 100;
            value = collection.Get<int>("key");
            Assert.IsNotNull(value);
            Assert.AreEqual(100, value);
            collection.Set("key", "string");
        }

        [Test]
        [ExpectedException(typeof(PropertyValidationException))]
        public void TestValueTypeSetInvalidTypeViaDictionary()
        {
            IPropertySetCollection collection = GetCollection();
            collection.Add("key", new IntSchema { AllowNull = true });
            int? value = collection.Get<int?>("key");
            Assert.IsFalse(value.HasValue);
            collection["key"] = 100;
            value = collection.Get<int>("key");
            Assert.IsNotNull(value);
            Assert.AreEqual(100, value);
            collection["key"] = "hello" ;
        }

        [Test]
        public void TestValueTypeSchemaValidationSuccess()
        {
            IPropertySetCollection collection = GetCollection();
            collection.Add("key", new IntSchema { AllowNull = true, MaxValue = 10, MinValue = 5 });
            collection.Set("key", (int?)6);
            int? value = collection.Get<int>("key");
            Assert.IsNotNull(value);
            Assert.AreEqual(6, value);

        }

        [Test]
        [ExpectedException(typeof(PropertyValidationException))]
        public void TestValueTypeSchemaValidationFailMinValue()
        {
            IPropertySetCollection collection = GetCollection();
            collection.Add("key", new IntSchema { AllowNull = true, MaxValue = 10, MinValue = 5 });
            collection.Set("key", (int?)3);
        }

        [Test]
        [ExpectedException(typeof(PropertyValidationException))]
        public void TestValueTypeSchemaValidationFailMaxValue()
        {
            IPropertySetCollection collection = GetCollection();
            collection.Add("key", new IntSchema { AllowNull = true, MaxValue = 10, MinValue = 5 });
            collection.Set("key", (int?)11);

        }

        #endregion

        #region IDictionary Tests
        [Test]
        public void TestContainsKey()
        {
            IPropertySetCollection collection = GetCollection();
            collection["key"] = "value";
            Assert.IsTrue(collection.ContainsKey("key"));
            Assert.IsFalse(collection.ContainsKey("key1"));
        }
        [Test]
        public void TestAddValue()
        {
            IPropertySetCollection collection = GetCollection();
            collection.Add("key", "value");
            object v = collection["key"];
            Assert.IsNotNull(v);
            Assert.AreEqual("value",v);
        }
        [Test]
        public void TestRemoveValue()
        {
            IPropertySetCollection collection = GetCollection();
            collection.Add("key", "value");
            object v = collection["key"];
            Assert.IsNotNull(v);
            Assert.AreEqual("value", v);
            Assert.IsTrue(collection.Remove("key"));
            try
            {
                v = collection["key"];
                Assert.Fail("Exception must be thrown.");
            }
            catch (KeyNotFoundException)
            {
                
            }
            

        }
        [Test]
        public void TestTryGetValue()
        {
            IPropertySetCollection collection = GetCollection();
            collection.Add("key", "value");
            object v;
            Assert.IsTrue(collection.TryGetValue("key",out v));
            Assert.IsNotNull(v);
            Assert.AreEqual("value",v);
            Assert.IsFalse(collection.TryGetValue("key1", out v));
            Assert.IsNull(v);

        }
        [Test]
        public void TestSetByKey()
        {
            IPropertySetCollection collection = GetCollection();
            collection["key"]= "value";
            object v = collection["key"];
            Assert.IsNotNull(v);
            Assert.AreEqual("value", v);
            collection["key"] = "value1";
            v = collection["key"];
            Assert.IsNotNull(v);
            Assert.AreEqual("value1", v);
        }
        public void TestGetByKey()
        {
            IPropertySetCollection collection = GetCollection();
            collection.Add("key", "value");
            object v = collection["key"];
            Assert.IsNotNull(v);
            Assert.AreEqual("value", v);
            v = collection["key1"];
            Assert.IsNull(v);
        }
        [Test]
        public void TestEnumerateKeys()
        {
            IPropertySetCollection collection = GetCollection();
            collection["1"] = 1;
            collection["2"] = 2;
            collection["3"] = 3;
            collection["4"] = 4;
            ICollection<string> keys = collection.Keys;
            Assert.IsNotNull(keys);
            Assert.AreEqual(4,keys.Count);
            string[] kv=new string[]{"1","2","3","4"};
            foreach (string key in keys)
            {
                if(!kv.Contains(key))                
                    Assert.Fail("The sequence contains invalid key:{0}",key);
            }
        }
        [Test]
        public void TestEnumerateValues()
        {
            IPropertySetCollection collection = GetCollection();
            collection["1"] = 1;
            collection["2"] = 2;
            collection["3"] = 3;
            collection["4"] = 4;
            ICollection<object> values = collection.Values;
            Assert.IsNotNull(values);
            Assert.AreEqual(4, values.Count);
            int?[] kv = new int?[]{ 1, 2, 3, 4 };
            foreach (object v in values)
            {
                if (!kv.Contains((int?)v))
                    Assert.Fail("The sequence contains invalid value:{0}", v);
            }
        }

        #endregion

        #region ICollection Tests
        [Test]
        public void TestAddKeyValuePair()
        {
            KeyValuePair<string, object> item=new KeyValuePair<string, object>("key","value");
            IPropertySetCollection collection = GetCollection();
            collection.Add(item);
            string s = collection.Get<string>("key");
            Assert.IsNotNull(s);
            Assert.AreEqual("value",s);
        }
        [Test]
        public void TestClearCollection()
        {
            IPropertySetCollection collection = GetCollection();
            collection["1"] = 1;
            collection["2"] = 2;
            collection["3"] = 3;
            collection["4"] = 4;
            Assert.AreEqual(4,collection.Count);
            collection.Clear();
            Assert.AreEqual(0, collection.Count);

        }
        [Test]
        public void TestContainsKvPair()
        {
            KeyValuePair<string, object> item = new KeyValuePair<string, object>("key", "value");
            IPropertySetCollection collection = GetCollection();
            collection.Add(item);
            Assert.IsTrue(collection.Contains(item));
            collection["key"] = "1";
            Assert.IsFalse(collection.Contains(item));
        }
        [Test]
        public void TestCopyToArray()
        {
            KeyValuePair<string, object>[] array=new KeyValuePair<string, object>[4];
            IPropertySetCollection collection = GetCollection();
            collection["1"] = "1";
            collection["2"] = "2";
            collection["3"] = "3";
            collection["4"] = "4";
            collection.CopyTo(array,0);
            foreach (KeyValuePair<string, object> kvp in array)
            {
                Assert.IsNotNull(kvp);
                object v = collection[kvp.Key];
                Assert.IsNotNull(v);
                Assert.AreEqual(kvp.Value,v);
            }
        }
        [Test]
        public void RemoveKeyValuePair()
        {
            KeyValuePair<string, object> item=new KeyValuePair<string, object>("1","1");
            IPropertySetCollection collection = GetCollection();
            collection["1"] = 1;
            Assert.AreEqual(1,collection.Count);
            
        }
        [Test]
        public void TestCollectionCount()
        {
            IPropertySetCollection collection = GetCollection();
            collection["1"] = 1;
            collection["2"] = 2;
            collection["3"] = 3;
            collection["4"] = 4;
            Assert.AreEqual(4, collection.Count);
        }
        [Test]
        public void TestIsCollectionReadOnly()
        {
            IPropertySetCollection collection = GetCollection();
            Assert.IsFalse(collection.IsReadOnly);
        }
        #endregion

        #region IEnumerable Tests
        [Test]
        public void TestGetEnumerator()
        {
            IPropertySetCollection collection = GetCollection();
            collection["1"] = 1;
            collection["2"] = 2;
            collection["3"] = 3;
            collection["4"] = 4;
            IEnumerator<KeyValuePair<string,object>> en= collection.GetEnumerator();
            Assert.IsNotNull(en);
            int count = 0;
            while(en.MoveNext())
            {
                count++;
                KeyValuePair<string, object> c = en.Current;
                Assert.IsNotNull(c);
            }
            Assert.AreEqual(4,count);
        }
        [Test]
        public void TestGetEnumerator2()
        {
            IPropertySetCollection collection = GetCollection();
            collection["1"] = 1;
            collection["2"] = 2;
            collection["3"] = 3;
            collection["4"] = 4;
            IEnumerator en = ((IEnumerable) collection).GetEnumerator();
            int count = 0;
            while (en.MoveNext())
            {
                count++;
                object c = en.Current;
                Assert.IsTrue(c is KeyValuePair<string, object>);
            }
            Assert.AreEqual(4, count);

        }

        #endregion

        #region Null Value and Object Value Tests
        
        [Test]
        public void TestAddNullValueNoSchema()
        {
            IPropertySetCollection collection = GetCollection();
            collection.Add("key", null);
            object value = collection.Get<object>("key");
            Assert.IsNull(value);
        }

        [Test]
        public void TestSetNullValueNoSchema()
        {
            IPropertySetCollection collection = GetCollection();
            collection["key"]=null;
            object value = collection.Get<object>("key");
            Assert.IsNull(value);
            value = collection["key"];
            Assert.IsNull(value);
        }

        [Test]
        public void TestAddObjectValueNoSchema()
        {
            IPropertySetCollection collection = GetCollection();
            Person p=new Person{FirstName = "John",LastName = "Doe"};
            collection.Add("key", p);
            Person pr = collection.Get<Person>("key");
            Assert.IsNotNull(pr);
            Assert.AreEqual(p.FirstName,pr.FirstName);
            Assert.AreEqual(p.LastName,pr.LastName);

        }

        [Test]
        public void TestSetObjectValueNoSchema()
        {
            IPropertySetCollection collection = GetCollection();
            Person p = new Person { FirstName = "John", LastName = "Doe" };
            collection["key"] = p;
            Person pr = collection.Get<Person>("key");
            Assert.IsNotNull(pr);
            Assert.AreEqual(p.FirstName, pr.FirstName);
            Assert.AreEqual(p.LastName, pr.LastName);
        }

        [Test]
        public void TestAddStringValueNoSchema()
        {
            IPropertySetCollection collection = GetCollection();
            collection.Add("key", "val");
            string value = collection.Get<string>("key");
            Assert.IsNotNull(value);
            Assert.AreEqual("val",value);

        }
        [Test]
        public void TestSetStringValueNoSchema()
        {
            IPropertySetCollection collection = GetCollection();
            collection["key"] = "val";
            string value = collection.Get<string>("key");
            Assert.IsNotNull(value);
            Assert.AreEqual("val", value);
            value = collection["key"] as string;
            Assert.IsNotNull(value);
            Assert.AreEqual("val", value);
        }

        #endregion

        /// <summary>
        /// Gets the values collection.
        /// </summary>
        /// <returns></returns>
        protected virtual IPropertySetCollection GetCollection()
        {
            return new MockPropertySetCollection(new MockPropertySchemaSet(new PropertySchemaFactory()));
        }
        internal class MockPropertySchemaSet:PropertySchemaSetBase
        {
            private readonly Dictionary<string, IValueSchema<object>> _schemaStorage = new Dictionary<string, IValueSchema<object>>();
            public MockPropertySchemaSet(IPropertySchemaFactory schemaFactory) : base(schemaFactory)
            {
            }

            #region Overrides of PropertySchemaSetBase

            /// <summary>
            /// Gets the schemas.
            /// </summary>
            public override IEnumerable<KeyValuePair<string, IValueSchema<object>>> Schemas
            {
                get { return _schemaStorage; }
            }

            /// <summary>
            /// Removes the schema.
            /// </summary>
            /// <param name="name">The name.</param>
            /// <returns></returns>
            public override bool RemoveSchema(string name)
            {
                return _schemaStorage.Remove(name);
            }

            /// <summary>
            /// Removes all schemas.
            /// </summary>
            public override void RemoveAll()
            {
                _schemaStorage.Clear();
            }

            /// <summary>
            /// Create or save the schema
            /// </summary>
            /// <param name="name">The name.</param>
            /// <param name="wrapped">The wrapped.</param>
            protected override void OnSetSchema(string name, IValueSchema<object> wrapped)
            {
                _schemaStorage[name] = wrapped;
            }

            /// <summary>
            /// Try to get the schema
            /// </summary>
            /// <param name="name">The name.</param>
            /// <param name="objSchema">The obj schema.</param>
            /// <returns></returns>
            protected override bool OnTryGetValue(string name, out IValueSchema<object> objSchema)
            {
                return _schemaStorage.TryGetValue(name, out objSchema);
            }

            #endregion
        }
        internal class MockPropertySetCollection:PropertySetCollectionBase
        {
            private readonly IPropertySchemaSet _schemas;
            private readonly Dictionary<string ,object > _store=new Dictionary<string, object>();

            public MockPropertySetCollection(IPropertySchemaSet schemas)
            {
                _schemas = schemas;
            }

            #region Overrides of PropertySetCollectionBase

            /// <summary>
            /// Gets the Properties Schema Set.
            /// </summary>
            /// <value>The schemas.</value>
            public override IPropertySchemaSet Schemas
            {
                get { return _schemas; }
            }

            /// <summary>
            /// Returns an enumerator that iterates through the collection.
            /// </summary>
            /// <returns>
            /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
            /// </returns>
            /// <filterpriority>1</filterpriority>
            public override IEnumerator<KeyValuePair<string, object>> GetEnumerator()
            {
                return _store.GetEnumerator();
            }

            /// <summary>
            /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
            /// </summary>
            /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
            public override void Add(KeyValuePair<string, object> item)
            {
                _store.Add(item.Key,item.Value);
            }

            /// <summary>
            /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
            /// </summary>
            /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only. </exception>
            public override void Clear()
            {
                _store.Clear();
            }

            /// <summary>
            /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
            /// </summary>
            /// <returns>
            /// true if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false.
            /// </returns>
            /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
            public override bool Contains(KeyValuePair<string, object> item)
            {
                return _store.Contains(item);
            }

            /// <summary>
            /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
            /// </summary>
            /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param><param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param><exception cref="T:System.ArgumentNullException"><paramref name="array"/> is null.</exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than 0.</exception><exception cref="T:System.ArgumentException"><paramref name="array"/> is multidimensional.-or-The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.-or-Type <paramref name="T"/> cannot be cast automatically to the type of the destination <paramref name="array"/>.</exception>
            public override void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
            /// </summary>
            /// <returns>
            /// true if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
            /// </returns>
            /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
            public override bool Remove(KeyValuePair<string, object> item)
            {
                return _store.Remove(item.Key);
            }

            /// <summary>
            /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
            /// </summary>
            /// <returns>
            /// The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
            /// </returns>
            public override int Count
            {
                get { return _store.Count; }
            }

            /// <summary>
            /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
            /// </summary>
            /// <returns>
            /// true if the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only; otherwise, false.
            /// </returns>
            public override bool IsReadOnly
            {
                get { return false; }
            }

            /// <summary>
            /// Determines whether the <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an element with the specified key.
            /// </summary>
            /// <returns>
            /// true if the <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an element with the key; otherwise, false.
            /// </returns>
            /// <param name="key">The key to locate in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.</param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception>
            public override bool ContainsKey(string key)
            {
                return _store.ContainsKey(key);
            }

            /// <summary>
            /// Adds an element with the provided key and value to the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
            /// </summary>
            /// <param name="key">The object to use as the key of the element to add.</param><param name="value">The object to use as the value of the element to add.</param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception><exception cref="T:System.ArgumentException">An element with the same key already exists in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.</exception><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IDictionary`2"/> is read-only.</exception>
            public override void Add(string key, object value)
            {
                _store.Add(key,value);
            }

            /// <summary>
            /// Removes the element with the specified key from the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
            /// </summary>
            /// <returns>
            /// true if the element is successfully removed; otherwise, false.  This method also returns false if <paramref name="key"/> was not found in the original <see cref="T:System.Collections.Generic.IDictionary`2"/>.
            /// </returns>
            /// <param name="key">The key of the element to remove.</param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IDictionary`2"/> is read-only.</exception>
            public override bool Remove(string key)
            {
                return _store.Remove(key);
            }

            /// <summary>
            /// Gets the value associated with the specified key.
            /// </summary>
            /// <returns>
            /// true if the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an element with the specified key; otherwise, false.
            /// </returns>
            /// <param name="key">The key whose value to get.</param><param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value"/> parameter. This parameter is passed uninitialized.</param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception>
            public override bool TryGetValue(string key, out object value)
            {
                return _store.TryGetValue(key, out value);
            }

            /// <summary>
            /// Gets or sets the element with the specified key.
            /// </summary>
            /// <returns>
            /// The element with the specified key.
            /// </returns>
            /// <param name="key">The key of the element to get or set.</param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception><exception cref="T:System.Collections.Generic.KeyNotFoundException">The property is retrieved and <paramref name="key"/> is not found.</exception><exception cref="T:System.NotSupportedException">The property is set and the <see cref="T:System.Collections.Generic.IDictionary`2"/> is read-only.</exception>
            public override object this[string key]
            {
                get { return _store[key]; }
                set { _store[key]=value; }
            }

            /// <summary>
            /// Gets an <see cref="T:System.Collections.Generic.ICollection`1"/> containing the keys of the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
            /// </summary>
            /// <returns>
            /// An <see cref="T:System.Collections.Generic.ICollection`1"/> containing the keys of the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/>.
            /// </returns>
            public override ICollection<string> Keys
            {
                get { return _store.Keys; }
            }

            /// <summary>
            /// Gets an <see cref="T:System.Collections.Generic.ICollection`1"/> containing the values in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
            /// </summary>
            /// <returns>
            /// An <see cref="T:System.Collections.Generic.ICollection`1"/> containing the values in the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/>.
            /// </returns>
            public override ICollection<object> Values
            {
                get { return _store.Values; }
            }

            #endregion
        }        
    }
}
