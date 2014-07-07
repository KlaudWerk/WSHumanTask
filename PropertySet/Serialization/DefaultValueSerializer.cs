/**
The MIT License (MIT)

Copyright (c) 2013 Igor Polouektov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
  */
using System;
using System.IO;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;

namespace Klaudwerk.PropertySet.Serialization
{
    /// <summary>
    /// Default implementation of a Value Serializer
    /// </summary>
    public class DefaultValueSerializer:IValueSerializer<object>
    {
        private readonly JsonSerializer _serializer = new JsonSerializer(); 
        private readonly BinaryFormatter _formatter=new BinaryFormatter();
        private readonly Type _type;
        /// <summary>
        /// Gets the capabilities.
        /// </summary>
        /// <value>The capabilities.</value>
        public SerializationCapabilities Capabilities
        {
            get { return SerializationCapabilities.Json | SerializationCapabilities.Binary; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultValueSerializer"/> class.
        /// </summary>
        public DefaultValueSerializer(Type type)
        {
            _formatter.AssemblyFormat = FormatterAssemblyStyle.Simple;
            _type = type;
        }
        /// <summary>
        /// Serializing of a primitive type
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="callBack">The call back.</param>
        public void Serialize(object value,IValueSerializationTarget callBack)
        {
            callBack.Set(ToJson(value), value.GetType());
        }

        /// <summary>
        /// Serialize an object to Binary serialization format
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public byte[] ToBinary(object value)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                _formatter.Serialize(ms, value);
                return ms.GetBuffer();
            }
        }

        /// <summary>
        /// Serialize an object 
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public string ToJson(object value)
        {
            using (StringWriter sw = new StringWriter())
            {
                _serializer.Serialize(sw,value);
                return sw.ToString();
            }
        }
        /// <summary>
        /// Deserializes the specified json string.
        /// </summary>
        /// <param name="jsonString">The json string.</param>
        /// <returns></returns>
        public object Deserialize(string jsonString)
        {
            using (StringReader sr = new StringReader(jsonString))
            {
                return _serializer.Deserialize(sr, _type);
            }
        }
        /// <summary>
        /// Deserializes the specified binary data.
        /// </summary>
        /// <param name="binaryData">The binary data.</param>
        /// <returns></returns>
        public object Deserialize(byte[] binaryData)
        {
            using(MemoryStream ms=new MemoryStream(binaryData))
            {
                return _formatter.Deserialize(ms);
            }
        }
    }
}
