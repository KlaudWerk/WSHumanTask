
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
using Klaudwerk.PropertySet.Serialization;

namespace Klaudwerk.PropertySet.Hibernate
{
    /// <summary>
    /// Property element mapped in NHibernate
    /// </summary>
    public class HibernatePropertyElement:PropertyElement,IValueSerializationTarget
    {
        /// <summary>
        /// Gets or sets the string value.
        /// </summary>
        /// <value>The string value.</value>
        public virtual string StringValue { get; set; }
        /// <summary>
        /// Gets or sets the int value.
        /// </summary>
        /// <value>The int value.</value>
        public virtual int? IntValue { get; set; }
        /// <summary>
        /// Gets or sets the long value.
        /// </summary>
        /// <value>The long value.</value>
        public virtual long? LongValue { get; set; }
        /// <summary>
        /// Gets or sets the double value.
        /// </summary>
        /// <value>The double value.</value>
        public virtual double? DoubleValue { get; set; }
        /// <summary>
        /// Gets or sets the date time value.
        /// </summary>
        /// <value>The date time value.</value>
        public virtual DateTime? DateTimeValue { get; set; }
        /// <summary>
        /// Gets or sets the raw value.
        /// </summary>
        /// <value>The raw value.</value>
        public virtual byte[] RawValue { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public override object Value
        {
            get
            {
                switch (SerializationHint)
                {
                    case SerializationTypeHint.Bool:
                        return IntValue.HasValue ? IntValue != 0 : false;
                    case SerializationTypeHint.Int:
                        return IntValue;
                    case SerializationTypeHint.Long:
                        return LongValue;
                    case SerializationTypeHint.Double:
                        return DoubleValue;
                    case SerializationTypeHint.DateTime:
                        return DateTimeValue;
                    case SerializationTypeHint.String:
                        return StringValue;
                    case SerializationTypeHint.ByteArray:
                        return RawValue;
                    case SerializationTypeHint.Null:
                        return null;
                    default:
                        return null;
                }
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void Set(int? value)
        {
            SerializationHint = SerializationTypeHint.Int;
            IntValue = value;
        }

        public void Set(long? value)
        {
            SerializationHint = SerializationTypeHint.Long;
            LongValue = value;
        }

        public void Set(double? value)
        {
            SerializationHint = SerializationTypeHint.Double;
            DoubleValue = value;
        }

        public void Set(bool? value)
        {
            SerializationHint = SerializationTypeHint.Bool;
            IntValue = (value.HasValue&&value.Value)?1:0;

        }

        public void Set(string value)
        {
            SerializationHint = SerializationTypeHint.String;
            StringValue = value;
        }

        public void Set(DateTime? value)
        {
            SerializationHint = SerializationTypeHint.DateTime;
            DateTimeValue = value;
        }

        public void Set(byte[] value)
        {
            SerializationHint = SerializationTypeHint.ByteArray;
            RawValue = value;
        }

        public void Set(byte[] value, Type type)
        {
            SerializationHint = SerializationTypeHint.BinaryObject;
            ValueType = type.FullName;
            RawValue = value;
        }

        public void Set(string value, Type type)
        {
            SerializationHint = SerializationTypeHint.JsonString;
            ValueType = type.FullName;
            StringValue = value;
        }
    }
}
