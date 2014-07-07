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
using System.Linq;

namespace Klaudwerk.PropertySet
{
    /// <summary>
    /// Extension methods for Value Schema
    /// </summary>
    public static class ValueSchemaExt
    {
        /// <summary>
        /// Wraps the specified schema.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="schema">The schema.</param>
        /// <returns></returns>
        public static IValueSchema<object> Wrap<T>(this IValueSchema<T> schema) where T:class
        {
            if (typeof(object).Equals(typeof(T)))
                return (IValueSchema<object>) schema;
            return new SchemaClassWrapper<T>(schema);
        }

        /// <summary>
        /// Wraps the specified schema.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="schema">The schema.</param>
        /// <returns></returns>
        public static IValueSchema<object> Wrap<T>(this IValueSchema<T?> schema) where T : struct
        {
            return new SchemaValueTypeWrapper<T>(schema);
        }

        /// <summary>
        /// Unwraps the reference type value schema
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="schema">The schema.</param>
        /// <returns></returns>
        public static IValueSchema<T> UnwrapRefType<T>(this IValueSchema<object> schema) where T : class
        {
            if (typeof(T) == typeof(object))
                return (IValueSchema<T>)schema;
            if(!typeof(T).IsAssignableFrom(schema.Type))
                throw new InvalidCastException(string.Format("Cannot convert between type of <T> {0} and {1} ",typeof(T).FullName,schema.Type.FullName));
            SchemaClassWrapper<T> wrapper = schema as SchemaClassWrapper<T>;
            if (wrapper == null)
                throw new ArgumentException("Cannot unwrap the schema of for the type {0}", schema.Type.FullName);
            return (IValueSchema<T>)wrapper.Real;
        }

        /// <summary>
        /// Unwraps the value type value schema.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="schema">The schema.</param>
        /// <returns></returns>
        public static IValueSchema<T?> UnwrapValueType<T>(this IValueSchema<object> schema) where T : struct
        {
            if (!typeof(T?).IsAssignableFrom(schema.Type))
                throw new InvalidCastException(string.Format("Cannot convert between type of <T> {0} and {1} ", typeof(T).FullName, schema.Type.FullName));
            SchemaValueTypeWrapper<T> wrapper = schema as SchemaValueTypeWrapper<T>;
            if(wrapper==null)
                throw new ArgumentException("Cannot unwrap the schema of for the type {0}",schema.Type.FullName);
            return (IValueSchema<T?>) wrapper.Real;
        }

        #region Private class
        /// <summary>
        /// The schema wrapper for Nullable Value types
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private class SchemaValueTypeWrapper<T> : IValueSchema<object> where T:struct 
        {
            private readonly IValueSchema<T?> _real;
            private readonly SerializerWrapper<T?> _serializer;
            public object Real
            {
                get { return _real; }
            }
            /// <summary>
            /// Initializes a new instance of the <see cref="SchemaValueTypeWrapper{T}"></see>
            ///   class.
            /// </summary>
            /// <param name="real">The real.</param>
            public SchemaValueTypeWrapper(IValueSchema<T?> real)
            {
                _real = real;
                _serializer=new SerializerWrapper<T?>(_real);
            }

            public void Validate(object value)
            {
                _real.Validate(value);
            }

            /// <summary>
            /// Converts the specified source to the schema's target type.
            /// </summary>
            /// <param name="source">The source.</param>
            /// <returns></returns>
            public object Convert(object source)
            {
                return _real.Convert(source);
            }

            public void Accept(IValueSchemaVisitor visitor)
            {
                _real.Accept(visitor);
            }

            public IEnumerable<object> PossibleValues
            {
                get { return _real.PossibleValues.Cast<object>(); }
                set
                {
                    _real.PossibleValues = value == null ? null : value.Cast<T?>();
                }
            }


            public bool AllowNull
            {
                get { return _real.AllowNull; }
                set { _real.AllowNull = value; }
            }

            public bool ReadOnly
            {
                get { return _real.ReadOnly; }
                set { _real.ReadOnly = value; }
            }

            public IValueSerializer<object> Serializer
            {
                get { return _serializer; }
                set { _serializer.SetValue(value); }
            }

            public bool HasDefault
            {
                get { return _real.HasDefault; }
                set { _real.HasDefault = value; }
            }

            public object DefaultValue
            {
                get { return _real.DefaultValue; }
                set { _real.DefaultValue = _real.Convert(value); }
            }

            public Type Type
            {
                get { return _real.Type; }
            }

            public override bool Equals(object obj)
            {
                return _real.Equals(obj);
            }

            public override int GetHashCode()
            {
                return _real.GetHashCode();
            }
        }

        private class SchemaClassWrapper<T>:IValueSchema<object> where T:class
        {
            public object Real
            {
                get { return _real; }
            }
            public void Validate(object value)
            {
                _real.Validate(value);
            }

            public object Convert(object source)
            {
                return _real.Convert(source);
            }

            public void Accept(IValueSchemaVisitor visitor)
            {
                _real.Accept(visitor);
            }

            private readonly IValueSchema<T> _real;
            private readonly SerializerWrapper<T> _serializerWrapper;

            /// <summary>
            /// Initializes a new instance of the <see cref="SchemaClassWrapper&lt;T&gt;"/> class.
            /// </summary>
            /// <param name="real">The real.</param>
            public SchemaClassWrapper(IValueSchema<T> real)
            {
                _real = real;
                _serializerWrapper = new SerializerWrapper<T>(_real);

            }

            public bool AllowNull
            {
                get { return _real.AllowNull; }
                set { _real.AllowNull = value; }
            }


            public bool ReadOnly
            {
                get { return _real.ReadOnly; }
                set { _real.ReadOnly = value; }
            }

            public IValueSerializer<object> Serializer
            {
                get { return _serializerWrapper; }
                set { _serializerWrapper.SetValue(value); }
            }

            public IEnumerable<object> PossibleValues
            {
                get { return _real.PossibleValues; }
                set { _real.PossibleValues = value==null?null:value.Cast<T>(); }
            }

            public bool HasDefault
            {
                get { return _real.HasDefault; }
                set { _real.HasDefault = value; }
            }

            public object DefaultValue
            {
                get { return _real.DefaultValue; }
                set { _real.DefaultValue = value as T; }
            }

            public Type Type
            {
                get { return _real.Type; }
            }

            public override bool Equals(object obj)
            {
                return _real.Equals(obj);
            }

            public override int GetHashCode()
            {
                return _real.GetHashCode();
            }
        }

        /// <summary>
        /// Wrapper class for Value Serializer
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private class SerializerWrapper<T>:IValueSerializer<object> 
        {
            private readonly IValueSchema<T> _real;


            public SerializerWrapper(IValueSchema<T> real)
            {
                _real = real;
            }

            /// <summary>
            /// Gets the capabilities.
            /// </summary>
            /// <value>The capabilities.</value>
            public SerializationCapabilities Capabilities
            {
                get { return _real.Serializer.Capabilities; }
            }

            /// <summary>
            /// Serializing of a primitive type
            /// </summary>
            /// <param name="value">The value.</param>
            /// <param name="callBack">The call back.</param>
            public void Serialize(object value, IValueSerializationTarget callBack)
            {
                T v = (T) value;
                _real.Serializer.Serialize(v,callBack);
            }

            /// <summary>
            /// Serialize an object to Binary serialization format
            /// </summary>
            /// <param name="value">The value.</param>
            /// <returns></returns>
            public byte[] ToBinary(object value)
            {
                T v = (T) value;
                return _real.Serializer.ToBinary(v);
            }

            /// <summary>
            /// Serialize an object 
            /// </summary>
            /// <param name="value">The value.</param>
            /// <returns></returns>
            public string ToJson(object value)
            {
                T v = (T)value;
                return _real.Serializer.ToJson(v);
            }

            /// <summary>
            /// Deserializes the specified json string.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="jsonString">The json string.</param>
            /// <returns></returns>
            public object Deserialize(string jsonString)
            {
                return _real.Serializer.Deserialize(jsonString);
            }
            /// <summary>
            /// Deserializes the specified binary data.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="binaryData">The binary data.</param>
            /// <returns></returns>
            public object Deserialize(byte[] binaryData)
            {
                return _real.Serializer.Deserialize(binaryData);
            }

            public void SetValue(IValueSerializer<object> value)
            {
                _real.Serializer = (IValueSerializer<T>) value;
            }
        }

        #endregion
    }
}