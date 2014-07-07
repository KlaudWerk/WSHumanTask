using System;
using NUnit.Framework;

namespace Klaudwerk.PropertySet.Test
{
    [TestFixture]
    public class SchemaTests
    {
        [Test]
        public void AssignmentTest()
        {
            IValueSchema<ValueType> s = (IValueSchema<ValueType>) new IntSchema();
        }
        [Test]
        public void TestStringSchemaValidValue()
        {
            StringSchema schema = new StringSchema();
            schema.Validate("hello");
        }
        [Test]
        [ExpectedException(typeof(PropertyValidationException))]
        public void TestStringSchemaSetInvalidValueType()
        {
            StringSchema schema=new StringSchema();
            schema.Validate(1);
        }

        [Test]
        public void TestStringSchemaMaxLengthValid()
        {
            StringSchema schema = new StringSchema {MaxLength = 5};
            schema.Validate(string.Empty);
            schema.Validate("1");
            schema.Validate("12");
            schema.Validate("123");
            schema.Validate("1234");
            schema.Validate("12345");
            
        }
        [Test]
        [ExpectedException(typeof(PropertyValidationException))]
        public void TestStringSchemaMaxLengthInvalid()
        {
            StringSchema schema = new StringSchema { MaxLength = 5 };
            schema.Validate(string.Empty);
            schema.Validate("1");
            schema.Validate("12");
            schema.Validate("123");
            schema.Validate("1234");
            schema.Validate("12345");
            schema.Validate("123456");
        }

        [Test]
        public void TestStringSchemaMinLengthValid()
        {
            StringSchema schema = new StringSchema { MinLength = 2 };
            schema.Validate("12");
            schema.Validate("123");
            schema.Validate("1234");
            schema.Validate("12345");
            schema.Validate("123456");
        }

        [Test]
        [ExpectedException(typeof(PropertyValidationException))]
        public void TestStringSchemaMinLengthInvalidEmpty()
        {
            StringSchema schema = new StringSchema { MinLength = 2 };
            schema.Validate(string.Empty);
        }

        [Test]
        [ExpectedException(typeof(PropertyValidationException))]
        public void TestStringSchemaMinLengthInvalidOne()
        {
            StringSchema schema = new StringSchema { MinLength = 2 };
            schema.Validate("1");
        }

        [Test]
        public void TestStringSchemaMinMaxLengthValid()
        {
            StringSchema schema = new StringSchema { MinLength = 2, MaxLength = 5};
            schema.Validate("12");
            schema.Validate("123");
            schema.Validate("1234");
            schema.Validate("12345");
        }

        [Test]
        [ExpectedException(typeof(PropertyValidationException))]
        public void TestStringSchemaMinMaxLengthInvalidMin()
        {
            StringSchema schema = new StringSchema { MinLength = 2, MaxLength = 5 };
            schema.Validate("1");
        }
        [Test]
        [ExpectedException(typeof(PropertyValidationException))]
        public void TestStringSchemaMinMaxLengthInvalidMax()
        {
            StringSchema schema = new StringSchema { MinLength = 2, MaxLength = 5};
            schema.Validate("12");
            schema.Validate("123");
            schema.Validate("1234");
            schema.Validate("12345");
            schema.Validate("123456");
        }

        [Test]
        public void TestStringSchemaDefaultValue()
        {
            new StringSchema {DefaultValue = "123"};
        }


        [Test]
        [ExpectedException(typeof(PropertyValidationException))]
        public void TestStringSchemaDefaultValueInvalidMin()
        {
            new StringSchema {MinLength = 2, MaxLength = 5, DefaultValue = "1"};
        }
        [Test]
        [ExpectedException(typeof(PropertyValidationException))]
        public void TestStringSchemaDefaultValueInvalidMax()
        {
            new StringSchema {MinLength = 2, MaxLength = 5, DefaultValue = "123456"};
        }
        [Test]
        public void TestStringSchemaDefaultValueInAllowedValues()
        {
            new StringSchema
            {
                PossibleValues = new[] { "one", "two", "three", "four" },
                DefaultValue = "two"
            };
        }
        [Test]
        [ExpectedException(typeof(PropertyValidationException))]
        public void TestStringSchemaDefaultValueNotInAllowedValues()
        {
            new StringSchema
            {
                PossibleValues = new[] { "one", "two", "three", "four" },
                DefaultValue = "five"
            };
        }

        [Test]
        public void TestStringSchemaAllowedValuesValid()
        {
            StringSchema schema = new StringSchema
                                      {
                                          PossibleValues = new[]{"one","two","three","four"}
                                      };
            schema.Validate("three");
        }

        [Test]
        [ExpectedException(typeof(PropertyValidationException))]
        public void TestStringSchemaAllowedValuesInvalid()
        {
            StringSchema schema = new StringSchema
            {
                PossibleValues = new[] { "one", "two", "three", "four" }
            };
            schema.Validate("five");
        }

        [Test]
        public void TestStringSchemaAllowNull()
        {
            StringSchema schema = new StringSchema {AllowNull = true};
            schema.Validate(null);
        }

        [Test]
        [ExpectedException(typeof(PropertyValidationException))]
        public void TestStringSchemaNoNullAllowed()
        {
            StringSchema schema = new StringSchema { AllowNull = false };
            schema.Validate(null);
        }

        [Test]
        public void TestStringSchemaEquality()
        {
            StringSchema s1=new StringSchema();
            StringSchema s2 = new StringSchema();  
            Assert.IsTrue(s1.Equals(s2));
            Assert.IsTrue(s2.Equals(s1));
            Assert.AreEqual(s1.GetHashCode(),s2.GetHashCode());
            s1=new StringSchema
            {
                DefaultValue = "two",
                MaxLength = 100,
                MinLength = 1,
                PossibleValues = new[] { "one", "two", "three", "four" }
            };
            s2 = new StringSchema
            {
                DefaultValue = "two",
                MaxLength = 100,
                MinLength = 1,
                PossibleValues = new[] { "one", "two", "three", "four" }
            };

            Assert.IsTrue(s1.Equals(s2));
            Assert.IsTrue(s2.Equals(s1));
            Assert.AreEqual(s1.GetHashCode(), s2.GetHashCode());

            s2 = new StringSchema
            {
                DefaultValue = "two",
                MaxLength = 100,
                MinLength = 1,
                PossibleValues = new[] { "one", "2", "three", "four" }
            };
            Assert.IsFalse(s1.Equals(s2));
            Assert.IsFalse(s2.Equals(s1));
            Assert.AreNotEqual(s1.GetHashCode(), s2.GetHashCode());

            s2=new StringSchema{AllowNull = true};

            Assert.IsFalse(s1.Equals(s2));
            Assert.IsFalse(s2.Equals(s1));
            Assert.AreNotEqual(s1.GetHashCode(), s2.GetHashCode());


        }


    }
}
