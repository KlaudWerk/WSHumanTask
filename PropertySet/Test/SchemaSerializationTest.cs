﻿using Klaudwerk.PropertySet.Serialization;
using NUnit.Framework;

namespace Klaudwerk.PropertySet.Test
{
    [TestFixture]
    public class SchemaSerializationTest
    {
        [Test]
        public void TestSerializeStringSchema()
        {
            IValueSchema<string> schema = new StringSchema {AllowNull = true, 
                DefaultValue = "abc",
                MaxLength = 100,
                MinLength = 2,
                PossibleValues = new[]{"abc","def"}
            };
            JsonSchemaSerializationVisitor visitor=new JsonSchemaSerializationVisitor();
            schema.Accept(visitor);
            Assert.AreEqual(typeof(string),visitor.ValueType);
            Assert.AreEqual(typeof(StringSchema),visitor.SchemaType);
            Assert.IsNotNull(visitor.JsonValue);
            Assert.AreNotEqual(0,visitor.JsonValue.Length);
        }
        [Test]
        public void TestSerializeIntSchema()
        {
            IValueSchema<int?> schema = new IntSchema
                                            {
                                                AllowNull = false,
                                                DefaultValue = 10,
                                                MaxValue = 100,
                                                MinValue = 2,
                                                PossibleValues = new int?[] {10, 20, 30, 100}
                                            };
            JsonSchemaSerializationVisitor visitor = new JsonSchemaSerializationVisitor();
            schema.Accept(visitor);
            Assert.AreEqual(typeof(int?), visitor.ValueType);
            Assert.AreEqual(typeof(IntSchema), visitor.SchemaType);
            Assert.IsNotNull(visitor.JsonValue);
            Assert.AreNotEqual(0, visitor.JsonValue.Length);
        }

        [Test]
        public void TestDeserializeStringSchema()
        {
            IValueSchema<string> schema = new StringSchema
            {
                AllowNull = true,
                DefaultValue = "abc",
                MaxLength = 100,
                MinLength = 2,
                PossibleValues = new[] { "abc", "def" }
            };
            JsonSchemaSerializationVisitor visitor = new JsonSchemaSerializationVisitor();
            schema.Accept(visitor);

            IValueSchema<object> vs=JsonSchemaDeserializer.Deserialize(visitor.SchemaType, visitor.JsonValue);
            Assert.IsNotNull(vs);
            Assert.AreEqual(typeof(string),vs.Type);
        }

        [Test]
        public void TestDeserializeIntSchema()
        {
            IValueSchema<int?> schema = new IntSchema
            {
                AllowNull = false,
                DefaultValue = 10,
                MaxValue = 100,
                MinValue = 2,
                PossibleValues = new int?[] { 10, 20, 30, 100 }
            };
            JsonSchemaSerializationVisitor visitor = new JsonSchemaSerializationVisitor();
            schema.Accept(visitor);
            IValueSchema<object> vs = JsonSchemaDeserializer.Deserialize(visitor.SchemaType, visitor.JsonValue);
            Assert.IsNotNull(vs);
            Assert.AreEqual(typeof(int?), vs.Type);
        }

        [Test]
        public void TestStringSerializeProperties()
        {
            JsonPropertySerializer serializer=new JsonPropertySerializer(new PropertySchemaFactory());
            StringSchema schema=new StringSchema{MaxLength = 100,MinLength = 1, DefaultValue = "hello"};
            PropertyElement element = serializer.Serialize("hello", schema.Wrap());
            Assert.IsNotNull(element);
            Assert.AreEqual(typeof(StringSchema).FullName,element.Schema.SchemaType);
            Assert.AreEqual(SerializationTypeHint.String,element.SerializationHint);
            Assert.AreEqual("hello",element.Value);
        }

        [Test]
        public void TestStringDeSerializeProperties()
        {
            JsonPropertySerializer serializer = new JsonPropertySerializer(new PropertySchemaFactory());
            StringSchema schema = new StringSchema { MaxLength = 100, MinLength = 1, DefaultValue = "hello" };
            PropertyElement element = serializer.Serialize("hello", schema.Wrap());
            Assert.IsNotNull(element);
            Assert.AreEqual(typeof(StringSchema).FullName, element.Schema.SchemaType);
            Assert.AreEqual(SerializationTypeHint.String, element.SerializationHint);
            Assert.AreEqual("hello", element.Value);

            IValueSchema<object> vs = serializer.DeserializeSchema(element.Schema);
            Assert.IsNotNull(vs);
            IValueSchema<string> strSchema = vs.UnwrapRefType<string>();
            Assert.IsNotNull(strSchema);
        }

    }

}
