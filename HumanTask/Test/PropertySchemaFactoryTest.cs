using System;
using HumanTask.ValueSet;
using NUnit.Framework;

namespace HumanTask.Test
{
    [TestFixture]
    public class PropertySchemaFactoryTest
    {
        [Test]
        public void TestCreateDefaultSchemasRegisteredTypes()
        {
            IPropertySchemaFactory f = new PropertySchemaFactory();
            IValueSchema<object> schema = f.Create(typeof (int));
            Assert.IsNotNull(schema);
            Assert.AreEqual(typeof(int?), schema.Type);
            schema = f.Create(typeof(int?));
            Assert.AreEqual(typeof(int?), schema.Type);
            Assert.IsNotNull(schema);
            Assert.AreEqual(typeof (int?), schema.Type);
            schema = f.Create(typeof(long));
            Assert.IsNotNull(schema);
            Assert.AreEqual(typeof(long?), schema.Type);
            schema = f.Create(typeof(long?));
            Assert.IsNotNull(schema);
            Assert.AreEqual(typeof(long?), schema.Type);
            schema = f.Create(typeof(DateTime));
            Assert.IsNotNull(schema);
            Assert.AreEqual(typeof(DateTime?), schema.Type);
            schema = f.Create(typeof(DateTime?));
            Assert.IsNotNull(schema);
            Assert.AreEqual(typeof(DateTime?), schema.Type);
            schema = f.Create(typeof(bool));
            Assert.IsNotNull(schema);
            Assert.AreEqual(typeof(bool?), schema.Type);
            schema = f.Create(typeof(bool));
            Assert.AreEqual(typeof(bool?), schema.Type);
            Assert.IsNotNull(schema);
            schema = f.Create(typeof(string));
            Assert.IsNotNull(schema);
            Assert.AreEqual(typeof(string), schema.Type);
        }
        [Test]
        public void TestCreateDefaultSchemaAnyType()
        {
            IPropertySchemaFactory f = new PropertySchemaFactory();
            IValueSchema<object> schema = f.Create(typeof(double));
            Assert.IsNotNull(schema);
            Assert.AreEqual(typeof(object),schema.Type);
            
        }
        [Test]
        public void TestRegisterSchemaForType()
        {
            IPropertySchemaFactory f = new PropertySchemaFactory();
            f.RegisterSchema(typeof(double), ()=>new IntSchema().Wrap());
            f.RegisterSchema(typeof(double?), () => new IntSchema().Wrap());
            IValueSchema<object> schema = f.Create(typeof(double));
            Assert.IsNotNull(schema);
            Assert.AreEqual(typeof(int?),schema.Type);

        }
    }
}
