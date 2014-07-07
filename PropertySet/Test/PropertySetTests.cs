using System;
using NUnit.Framework;

namespace Klaudwerk.PropertySet.Test
{
    [TestFixture]
    public class PropertySetTests
    {

        [Test]
        public void TestPropertyAddIntNoSchema()
        {
            IPropertySetCollection provider = GetValueProvider();
            provider.Set("n",(int?)100);
            provider.Set("n","hello");

            provider.Add("val",100,new IntSchema());
            provider.Add("val1", "100", new StringSchema());
           
            int? result=provider.Get<int?>("val");
            Assert.IsNotNull(result);
            Assert.AreEqual(100,result.Value);
            provider["val"] = 300;
            object val = provider["val"];
            Assert.IsNotNull(val);
            result = val as int?;
            Assert.IsNotNull(result);
            Assert.AreEqual(300, result.Value);
        }

        [Test]
        public void TestPropertyAddIntWithSchemaDefaultVal()
        {
            IPropertySetCollection provider = GetValueProvider();
            provider.Add("val1", new IntSchema
            {
                DefaultValue = 40
            });
            int? result = provider.Get<int?>("val1");
            Assert.IsNotNull(result);
            Assert.AreEqual(40,result.Value);
            object val = provider["val1"];
            Assert.IsNotNull(val);
            result = val as int?;
            Assert.IsNotNull(result);
            Assert.AreEqual(40, result.Value);
        }

        [Test]
        public void TestPropertyAddIntWithSchemaValidation()
        {

        }

        [Test]
        public void TestPropertyAddIntDefaultSchemaAddValidation()
        {
        }

        protected virtual IPropertySetCollection GetValueProvider()
        {
            throw new NotImplementedException();
        }
    }
}
