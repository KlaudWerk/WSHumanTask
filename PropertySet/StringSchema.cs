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
using System.Text;

namespace Klaudwerk.PropertySet
{
    /// <summary>
    /// The schema validator for a string
    /// </summary>
    [Serializable]
    [DataContract]
    public class StringSchema:SchemaBase<string>
    {
        [DataMember]
        public override string DefaultValue
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
        public override IEnumerable<string> PossibleValues { get;  set; }

        /// <summary>
        /// Gets or sets the string minimum length
        /// </summary>
        /// <value>The min length of the string.</value>
        [DataMember]
        public int MinLength { get; set; }
        /// <summary>
        /// Gets or sets the string maximum length
        /// </summary>
        /// <value>The max length of the string.</value>
        [DataMember]
        public int MaxLength { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringSchema"/> class.
        /// </summary>
        public StringSchema():this(0,int.MaxValue)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringSchema"/> class.
        /// </summary>
        /// <param name="minLength">Length of the min.</param>
        /// <param name="maxLength">Length of the max.</param>
        public StringSchema(int minLength, int maxLength)
        {
            MinLength = minLength;
            MaxLength = maxLength;
            Serializer = new StringSerializer();
        }

        /// <summary>
        /// Validates the specified value on set.
        /// </summary>
        /// <param name="value">The value.</param>
        protected override string OnValidate(object value)
        {
            string str = System.Convert.ToString(value);
            if(str==null)
                throw new PropertyValidationException("value");
            if (str.Length < MinLength || str.Length > MaxLength)
                throw new PropertyValidationException("max");
            return str;
        }

        /// <summary>
        /// Converts the specified source to the schema's target type.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public override string Convert(object source)
        {
            return source == null ? null : ((source is string) ? source as string : source.ToString());
        }

        /// <summary>
        /// Accepts the specified visitor.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        public override void Accept(IValueSchemaVisitor visitor)
        {
            visitor.Visit(this);
        }

        /// <summary>
        /// Ares the values equal.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        protected override bool AreValuesEqual(string value, string other)
        {
            return string.Equals(value, other);
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
            if (ReferenceEquals(this, obj))
                return true;
            StringSchema other = obj as StringSchema;
            if (other == null)
                return false;
            return SchemasEqual(other) && 
                MaxLength==other.MaxLength && 
                MinLength==other.MinLength;
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
                int hashCode = 17 + MinLength.GetHashCode() ^ MaxLength.GetHashCode() ^ AllowNull.GetHashCode() ^
                               ReadOnly.GetHashCode()+31*GetHashCode(PossibleValues);
                return (DefaultValue == null) ? hashCode : hashCode*32 + DefaultValue.GetHashCode();
            }
        }
        /// <summary>
        /// Default string serializer
        /// </summary>
        private class StringSerializer:IValueSerializer<string>
        {
            /// <summary>
            /// Gets the capabilities.
            /// </summary>
            /// <value>The capabilities.</value>
            public SerializationCapabilities Capabilities
            {
                get
                {
                    return SerializationCapabilities.Binary | SerializationCapabilities.Json |
                           SerializationCapabilities.PrimitiveValue;
                }
            }

            /// <summary>
            /// Serializing of a primitive type
            /// </summary>
            /// <param name="value">The value.</param>
            /// <param name="callBack">The call back.</param>
            public void Serialize(string value, IValueSerializationTarget callBack)
            {
                callBack.Set(value);
            }

            /// <summary>
            /// Serialize an object to Binary serialization format
            /// </summary>
            /// <param name="value">The value.</param>
            /// <returns></returns>
            public byte[] ToBinary(string value)
            {
                return Encoding.Unicode.GetBytes(value);
            }

            /// <summary>
            /// Serialize an object 
            /// </summary>
            /// <param name="value">The value.</param>
            /// <returns></returns>
            public string ToJson(string value)
            {
                return  value;
            }

            /// <summary>
            /// Deserializes the specified json string.
            /// </summary>
            /// <param name="jsonString">The json string.</param>
            /// <returns></returns>
            public string Deserialize(string jsonString)
            {
                return jsonString;
            }

            /// <summary>
            /// Deserializes the specified json string.
            /// </summary>
            /// <param name="jsonString">The json string.</param>
            /// <param name="jsonType">Type of the json.</param>
            /// <returns></returns>
            public string Deserialize(string jsonString, Type jsonType)
            {
                return Deserialize(jsonString);
            }

            /// <summary>
            /// Deserializes the specified binary data.
            /// </summary>
            /// <param name="binaryData">The binary data.</param>
            /// <returns></returns>
            public string Deserialize(byte[] binaryData)
            {
                return Encoding.Unicode.GetString(binaryData);
            }
        }
    }
}
    

