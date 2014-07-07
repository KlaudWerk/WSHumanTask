using System;
using System.Collections.Generic;
using HumanTask.ValueSet;
using Moq;
using Moq.Protected;
using NUnit.Framework;

namespace HumanTask.Test
{
    [TestFixture]
    public class ValueSetCollectionTest
    {
        private readonly Dictionary<string, IValueSchema<object>> _schemaStorage = new Dictionary<string, IValueSchema<object>>();
        private readonly Dictionary<string, object> _valueStorage = new Dictionary<string, object>();

        [TearDown]
        public void TearDown()
        {
            _schemaStorage.Clear();
            _valueStorage.Clear();
        }

        #region Class (string) tests
        [Test]
        public void TestAddStringPropertyWithDefaultValue()
        {
            IValueSetCollection collection = GetCollection();
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
            IValueSetCollection collection = GetCollection();
            collection.Add("key", new StringSchema{AllowNull = true});
            string value = collection.Get<string>("key");
            Assert.IsNull(value);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestAddStringPropertyNoDefaultValueNotAllowNull()
        {
            IValueSetCollection collection = GetCollection();
            collection.Add("key", new StringSchema());
        }

        [Test]
        public void TestAddStringPropertyNoDefaultValueNotAllowNullWithValue()
        {
            IValueSetCollection collection = GetCollection();
            collection.Add("key","value", new StringSchema());
            string value = collection.Get<string>("key");
            Assert.IsNotNull(value);
            Assert.AreEqual("value", value);
        }

        [Test]
        public void TestStringSetValue()
        {
            IValueSetCollection collection = GetCollection();
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
            IValueSetCollection collection = GetCollection();
            collection.Add("key", new StringSchema { AllowNull = true });
            collection.Set("key", "value");
            object value = collection["key"];
            Assert.IsNotNull(value);
            Assert.AreEqual("value", value);
        }
        [Test]
        public void TestStringSetValueViaDictionary()
        {
            IValueSetCollection collection = GetCollection();
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
        public void TestAddStringOutsideOfTheistOfAllowedValues()
        {
            IValueSetCollection collection = GetCollection();
            collection.Add("key", new StringSchema {AllowNull = true, PossibleValues = new[] {"one", "two", "tree"}});
            collection.Set("key","1");
        }

        [Test]
        [ExpectedException(typeof(PropertyValidationException))]
        public void TestAddStringOutsideOfTheistOfAllowedValuesViaDictionary()
        {
            IValueSetCollection collection = GetCollection();
            collection.Add("key", new StringSchema { AllowNull = true, PossibleValues = new[] { "one", "two", "tree" } });
            collection["key"]="1";
        }

        [Test]
        [ExpectedException(typeof(PropertyValidationException))]
        public void TestStringSetInvalidType()
        {
            IValueSetCollection collection = GetCollection();
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
            IValueSetCollection collection = GetCollection();
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
            IValueSetCollection collection = GetCollection();
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
            IValueSetCollection collection = GetCollection();
            collection.Add("key", new StringSchema { AllowNull = true, MaxLength = 10, MinLength = 5 });
            collection.Set("key", "123");
        }

        [Test]
        [ExpectedException(typeof(PropertyValidationException))]
        public void TestStringSchemaValidationFailMaxValue()
        {
            IValueSetCollection collection = GetCollection();
            collection.Add("key", new StringSchema { AllowNull = true, MaxLength = 10, MinLength = 5 });
            collection.Set("key", "123456789012345");

        }
        #endregion

        #region Value Type tests
        [Test]
        public void TestAddValueTypePropertyWithDefaultValue()
        {
            IValueSetCollection collection = GetCollection();
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
            IValueSetCollection collection = GetCollection();
            collection.Add("key", new IntSchema { AllowNull = true });
            int? value = collection.Get<int?>("key");
            Assert.IsFalse(value.HasValue);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestAddValueTypePropertyNoDefaultValueNotAllowNull()
        {
            IValueSetCollection collection = GetCollection();
            collection.Add("key", new IntSchema());
        }

        [Test]
        public void TestAddValueTypePropertyNoDefaultValueNotAllowNullWithValue()
        {
            IValueSetCollection collection = GetCollection();
            collection.Add("key", 100, new IntSchema());
            int? value = collection.Get<int?>("key");
            Assert.IsNotNull(value);
            Assert.AreEqual(100, value);
        }

        [Test]
        public void TestValueTypeSetValue()
        {
            IValueSetCollection collection = GetCollection();
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
            IValueSetCollection collection = GetCollection();
            collection.Add("key", new IntSchema{ AllowNull = true });
            collection.Set("key", (int?)100);
            object value = collection["key"];
            Assert.IsNotNull(value);
            Assert.AreEqual(100, value);
        }
        [Test]
        public void TestValueTypeSetValueViaDictionary()
        {
            IValueSetCollection collection = GetCollection();
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
            IValueSetCollection collection = GetCollection();
            collection.Add("key", new IntSchema { AllowNull = true, PossibleValues = new int?[] { 2, 3, 4 } });
            collection.Set("key", (int?)1);
        }

        [Test]
        [ExpectedException(typeof(PropertyValidationException))]
        public void TestAddValueTypeOutsideOfTheSetOfAllowedValuesViaDictionary()
        {
            IValueSetCollection collection = GetCollection();
            collection.Add("key", new IntSchema { AllowNull = true, PossibleValues = new int?[] { 2, 3, 4 } });
            collection["key"]=1;
        }

        [Test]
        [ExpectedException(typeof(PropertyValidationException))]
        public void TestValueTypeSetInvalidType()
        {
            IValueSetCollection collection = GetCollection();
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
            IValueSetCollection collection = GetCollection();
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
            IValueSetCollection collection = GetCollection();
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
            IValueSetCollection collection = GetCollection();
            collection.Add("key", new IntSchema { AllowNull = true, MaxValue = 10, MinValue = 5 });
            collection.Set("key", (int?)3);
        }

        [Test]
        [ExpectedException(typeof(PropertyValidationException))]
        public void TestValueTypeSchemaValidationFailMaxValue()
        {
            IValueSetCollection collection = GetCollection();
            collection.Add("key", new IntSchema { AllowNull = true, MaxValue = 10, MinValue = 5 });
            collection.Set("key", (int?)11);

        }

        #endregion

        #region IDictionary Tests
        #endregion

        #region ICollection Tests
        #endregion

        #region IEnumerable Tests
        #endregion


        /// <summary>
        /// Gets the values collection.
        /// </summary>
        /// <returns></returns>
        protected virtual IValueSetCollection GetCollection()
        {
            Mock<PropertySchemaSetBase> mockSchemaSet = new Mock<PropertySchemaSetBase>(new PropertySchemaFactory()) { CallBase = true };
            mockSchemaSet.Protected().SetupGet<IDictionary<string, IValueSchema<object>>>("SchemaStore").Returns(_schemaStorage);

            Mock<ValueSetCollectionBase>  collectionMock=new Mock<ValueSetCollectionBase> {CallBase = true};
            collectionMock.SetupGet(s=>s.Schemas).Returns(mockSchemaSet.Object);
            collectionMock.Protected().SetupGet<IDictionary<string, object>>("Storage").Returns(_valueStorage);
            return collectionMock.Object;
        }
    }
}
