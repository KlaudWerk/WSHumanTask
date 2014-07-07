using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HumanTask.ValueSet;
using NUnit.Framework;

namespace HumanTask.Test
{
    [TestFixture]
    public class PropertySetTests
    {

        [Test]
        public void TestPropertyAddIntNoSchema()
        {
            IValueSetCollection provider = GetValueProvider();
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
            IValueSetCollection provider = GetValueProvider();
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

        protected virtual IValueSetCollection GetValueProvider()
        {
            throw new NotImplementedException();
        }
    }
}
