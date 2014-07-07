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
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Klaudwerk.PropertySet
{
    [Serializable]
    [DataContract]
    public class BoolSchema:SchemaBase<bool?>
    {
        private static readonly bool?[] _possibleValues = new bool?[] {true, false};
        [DataMember]
        public override bool? DefaultValue
        {
            get
            {
                return base.DefaultValue;
            }
            set
            {
                base.DefaultValue = value;
            }
        }
        /// <summary>
        /// Gets the list of possible values.
        /// </summary>
        /// <value>The possible values.</value>
        [DataMember]
        public override IEnumerable<bool?> PossibleValues
        {
            get { return _possibleValues; }
            set {}
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoolSchema"/> class.
        /// </summary>
        public BoolSchema()
        {
            Serializer = new BoolSerializer();
        }
        /// <summary>
        /// Validates the specified value on set.
        /// </summary>
        /// <param name="value">The value.</param>
        protected override bool? OnValidate(object value)
        {
            return Convert(value);
        }
        /// <summary>
        /// Converts the specified source to the schema's target type.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public override bool? Convert(object source)
        {
            return System.Convert.ToBoolean(source);
        }

        public override void Accept(IValueSchemaVisitor visitor)
        {
            visitor.Visit(this);
        }

        /// <summary>
        /// Check if two values of T are equals
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        protected override bool AreValuesEqual(bool? value, bool? other)
        {
            return value == other;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this,obj))
                return true;
            BoolSchema other = obj as BoolSchema;
            if (other == null)
                return false;
            return SchemasEqual(other);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = 17 + AllowNull.GetHashCode() ^ ReadOnly.GetHashCode()+31*GetHashCode(PossibleValues);;
                return (DefaultValue == null) ? hashCode : hashCode*32 ^ DefaultValue.GetHashCode();
            }
        }

        private class BoolSerializer:IValueSerializer<bool?>
        {
            /// <summary>
            /// Gets the capabilities.
            /// </summary>
            /// <value>The capabilities.</value>
            public SerializationCapabilities Capabilities
            {
                get
                {
                    return SerializationCapabilities.PrimitiveValue | SerializationCapabilities.Binary |
                           SerializationCapabilities.Json;
                }
            }

            /// <summary>
            /// Serializing of a primitive type
            /// </summary>
            /// <param name="value">The value.</param>
            /// <param name="callBack">The call back.</param>
            public void Serialize(bool? value, IValueSerializationTarget callBack)
            {
                callBack.Set(value);
            }

            /// <summary>
            /// Serialize an object to Binary serialization format
            /// </summary>
            /// <param name="value">The value.</param>
            /// <returns></returns>
            public byte[] ToBinary(bool? value)
            {
                return new []{(value.HasValue && value.Value)?(byte)0x1:(byte)0x0};
            }

            /// <summary>
            /// Serialize an object 
            /// </summary>
            /// <param name="value">The value.</param>
            /// <returns></returns>
            public string ToJson(bool? value)
            {
                return (value.HasValue && value.Value) ? "true" : "false";
            }

            /// <summary>
            /// Deserializes the specified json string.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="jsonString">The json string.</param>
            /// <returns></returns>
            public bool? Deserialize(string jsonString)
            {
                switch (jsonString)
                {
                    case "true":
                        return true;
                    case "false":
                        return false;
                    default:
                        return null;
                }
            }

            /// <summary>
            /// Deserializes the specified json string.
            /// </summary>
            /// <param name="jsonString">The json string.</param>
            /// <param name="jsonType">Type of the json.</param>
            /// <returns></returns>
            public bool? Deserialize(string jsonString, Type jsonType)
            {
                return Deserialize(jsonString);
            }

            /// <summary>
            /// Deserializes the specified binary data.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="binaryData">The binary data.</param>
            /// <returns></returns>
            public bool? Deserialize(byte[] binaryData)
            {
                int i = 0;
                i |= binaryData[0];
                return i == 1;
            }
        }
    }
}