using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Moq.Protected;
using NUnit.Framework;

namespace Klaudwerk.PropertySet.Test
{
    [TestFixture]
    public class PropertySchemaSetTest
    {
        private readonly Dictionary<string,IValueSchema<object>> _storage = new Dictionary<string, IValueSchema<object>>();

        [TearDown]
        public void TearDown()
        {
            _storage.Clear();
        }

        [Test]
        public void TestGetDefaultSchemas()
        {
            IPropertySchemaSet schemaSet = GetSchemaSet();
            IValueSchema<object> schema = schemaSet.GetDefaultSchema(typeof(int));
            Assert.IsNotNull(schema);
            Assert.AreEqual(typeof(int?), schema.Type);
            schema = schemaSet.GetDefaultSchema(typeof(int?));
            Assert.AreEqual(typeof(int?), schema.Type);
            Assert.IsNotNull(schema);
            Assert.AreEqual(typeof(int?), schema.Type);
            schema = schemaSet.GetDefaultSchema(typeof(long));
            Assert.IsNotNull(schema);
            Assert.AreEqual(typeof(long?), schema.Type);
            schema = schemaSet.GetDefaultSchema(typeof(long?));
            Assert.IsNotNull(schema);
            Assert.AreEqual(typeof(long?), schema.Type);
            schema = schemaSet.GetDefaultSchema(typeof(DateTime));
            Assert.IsNotNull(schema);
            Assert.AreEqual(typeof(DateTime?), schema.Type);
            schema = schemaSet.GetDefaultSchema(typeof(DateTime?));
            Assert.IsNotNull(schema);
            Assert.AreEqual(typeof(DateTime?), schema.Type);
            schema = schemaSet.GetDefaultSchema(typeof(bool));
            Assert.IsNotNull(schema);
            Assert.AreEqual(typeof(bool?), schema.Type);
            schema = schemaSet.GetDefaultSchema(typeof(bool));
            Assert.AreEqual(typeof(bool?), schema.Type);
            Assert.IsNotNull(schema);
            schema = schemaSet.GetDefaultSchema(typeof(string));
            Assert.IsNotNull(schema);
            Assert.AreEqual(typeof(string), schema.Type);
        }

        [Test]
        public void TestTryGetSetClassSchema()
        {
            IPropertySchemaSet schemaSet = GetSchemaSet();
            IValueSchema<string> s;
            Assert.IsFalse(schemaSet.TryGetSchema("string", out s));
            schemaSet.SetSchema("string",new StringSchema());
            Assert.IsTrue(schemaSet.TryGetSchema("string", out s));
            Assert.IsNotNull(s);
            Assert.AreEqual(typeof(string),s.Type);
        }

        [Test]
        public void TestTryGetSetValueTypeSchema()
        {
            IPropertySchemaSet schemaSet = GetSchemaSet();
            IValueSchema<int?> s;
            Assert.IsFalse(schemaSet.TryGetSchema("int", out s));
            schemaSet.SetSchema("int", new IntSchema());
            Assert.IsTrue(schemaSet.TryGetSchema("int", out s));
            Assert.IsNotNull(s);
            Assert.AreEqual(typeof(int?), s.Type);
        }

        [Test]
        public void TestEnumAllSchemas()
        {
            IPropertySchemaSet schemaSet = GetSchemaSet();
            schemaSet.SetSchema("string", new StringSchema());
            schemaSet.SetSchema("int", new IntSchema());
            IEnumerable<KeyValuePair<string,IValueSchema<object>>> sc=schemaSet.Schemas;
            Assert.IsNotNull(sc);
            Assert.AreEqual(2,sc.Count());
            foreach (KeyValuePair<string, IValueSchema<object>> pair in sc)
            {
                switch(pair.Key)
                {
                    case "string":
                        break;
                    case "int":
                        break;
                    default:
                        Assert.Fail("Unexpected schema name:"+pair.Key);
                        break;
                }
            }
        }

        [Test]
        public void TestRemoveSchema()
        {
            IPropertySchemaSet schemaSet = GetSchemaSet();
            IValueSchema<int?> s;
            Assert.IsFalse(schemaSet.TryGetSchema("int", out s));
            schemaSet.SetSchema("int", new IntSchema());
            Assert.IsTrue(schemaSet.TryGetSchema("int", out s));
            Assert.IsNotNull(s);
            Assert.AreEqual(typeof(int?), s.Type);
            schemaSet.RemoveSchema("int");
            Assert.IsFalse(schemaSet.TryGetSchema("int", out s));
        }

        [Test]
        public void TestRemoveAllSchemas()
        {
            IPropertySchemaSet schemaSet = GetSchemaSet();
            schemaSet.SetSchema("string", new StringSchema());
            schemaSet.SetSchema("int", new IntSchema());
            Assert.IsNotNull(schemaSet.Schemas);
            Assert.AreEqual(2,schemaSet.Schemas.Count());
            schemaSet.RemoveAll();
            Assert.IsNotNull(schemaSet.Schemas);
            Assert.AreEqual(0, schemaSet.Schemas.Count());
        }

        [Test]
        public void TestReplaceSchema()
        {
            IPropertySchemaSet schemaSet = GetSchemaSet();
            schemaSet.SetSchema("string", new StringSchema());
            IValueSchema<string> s;
            Assert.IsTrue(schemaSet.TryGetSchema("string", out s));
            Assert.IsNotNull(s);
            IValueSchema<int?> i;
            schemaSet.SetSchema("string", new IntSchema());
            Assert.IsTrue(schemaSet.TryGetSchema("string", out i));
        }


        [Test]
        public void TestGetSchemaType()
        {
            IPropertySchemaSet schemaSet = GetSchemaSet();
            schemaSet.SetSchema("string", new StringSchema());
            Type t = schemaSet.GetSchemaType("string");
            Assert.IsNotNull(t);
            Assert.AreEqual(typeof(string),t);
        }


        protected virtual IPropertySchemaSet GetSchemaSet()
        {
            return new ValueSetCollectionTest.MockPropertySchemaSet(new PropertySchemaFactory());
        }
    }
}
