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
using System.IO;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Klaudwerk.PropertySet
{
    /// <summary>
    /// Date Time schema
    /// </summary>
    [Serializable]
    [DataContract]
    public class DateTimeSchema:SchemaBase<DateTime?>
    {
        [DataMember]
        public override DateTime? DefaultValue
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
        /// Gets or sets the min value.
        /// </summary>
        /// <value>The min value.</value>
        [DataMember]
        public DateTime MinValue { get; set; }
        /// <summary>
        /// Gets or sets the max value.
        /// </summary>
        /// <value>The max value.</value>
        [DataMember]
        public DateTime MaxValue { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeSchema"/> class.
        /// </summary>
        public DateTimeSchema()
        {
            MinValue = DateTime.MinValue;
            MaxValue = DateTime.MaxValue;
            Serializer = new DateTimeSerializer();
        }

        /// <summary>
        /// Gets the list of possible values.
        /// </summary>
        /// <value>The possible values.</value>
        [DataMember]
        public override IEnumerable<DateTime?> PossibleValues
        {
            get; set;
        }

        /// <summary>
        /// Perrform type-specific validation
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        protected override DateTime? OnValidate(object value)
        {
            DateTime? t = Convert(value);
            if(MinValue.CompareTo(t)>0 || MaxValue.CompareTo(t)<0)
                throw new PropertyValidationException("minmax");
            return t;
        }

        /// <summary>
        /// Converts the specified source to the schema's target type.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public override DateTime? Convert(object source)
        {
            return System.Convert.ToDateTime(source);
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
        protected override bool AreValuesEqual(DateTime? value, DateTime? other)
        {
            return Equals(value, other);
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
            DateTimeSchema other = obj as DateTimeSchema;
            if (other == null)
                return false;
            return SchemasEqual(other) && MaxValue == other.MaxValue && MinValue == other.MinValue;

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
                int hashCode = 17 + MaxValue.GetHashCode() ^ MinValue.GetHashCode() ^ AllowNull.GetHashCode() ^
                               ReadOnly.GetHashCode() + 31 * GetHashCode(PossibleValues);
                return (DefaultValue == null) ? hashCode : hashCode * 32 + DefaultValue.GetHashCode();
            }
        }

        private class DateTimeSerializer:IValueSerializer<DateTime?>
        {
            private static readonly JsonSerializer _serializer = new JsonSerializer(); 
            /// <summary>
            /// Gets the capabilities.
            /// </summary>
            /// <value>The capabilities.</value>
            public SerializationCapabilities Capabilities
            {
                get 
                {
                    return SerializationCapabilities.Binary | SerializationCapabilities.Json 
                        |SerializationCapabilities.PrimitiveValue;
                }
            }

            /// <summary>
            /// Serializing of a primitive type
            /// </summary>
            /// <param name="value">The value.</param>
            /// <param name="callBack">The call back.</param>
            public void Serialize(DateTime? value, IValueSerializationTarget callBack)
            {
                callBack.Set(value);
            }

            /// <summary>
            /// Serialize an object to Binary serialization format
            /// </summary>
            /// <param name="value">The value.</param>
            /// <returns></returns>
            public byte[] ToBinary(DateTime? value)
            {
                byte[] serialized=new byte[]{0,0,0,0,0,0,0,0};
                if(value.HasValue)
                {
                    long v = value.Value.Ticks;
                    for (int i = 0; i < serialized.Length; i++)
                        serialized[i] = (byte)(v>> (i * 8) & 0xFF);
                }
                return serialized;
            }

            /// <summary>
            /// Serialize an object 
            /// </summary>
            /// <param name="value">The value.</param>
            /// <returns></returns>
            public string ToJson(DateTime? value)
            {
                using(StringWriter sw=new StringWriter())
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
            public DateTime? Deserialize(string jsonString)
            {
                using (StringReader sr=new StringReader(jsonString))
                {
                    return (DateTime?) _serializer.Deserialize(sr, typeof (DateTime?));
                }
            }

            /// <summary>
            /// Deserializes the specified json string.
            /// </summary>
            /// <param name="jsonString">The json string.</param>
            /// <param name="jsonType">Type of the json.</param>
            /// <returns></returns>
            public DateTime? Deserialize(string jsonString, Type jsonType)
            {
                return Deserialize(jsonString);
            }

            /// <summary>
            /// Deserializes the specified binary data.
            /// </summary>
            /// <param name="binaryData">The binary data.</param>
            /// <returns></returns>
            public DateTime? Deserialize(byte[] binaryData)
            {
                long ticks = 0;
                if (binaryData != null)
                {
                    for (int i = 0; i < binaryData.Length; i++)
                    {
                        if (i >= 31)
                            break;
                        ticks |= ((long)binaryData[i]) << (i << 3);
                    }
                }
                return new DateTime(ticks);
            }
        }
    }
}
