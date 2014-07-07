using System;
using System.Collections.Generic;
using System.Linq;

namespace HumanTask.ValueSet
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

            public IValueSerializer Serializer
            {
                get { return _real.Serializer; }
                set { _real.Serializer = value; }
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

            public SchemaClassWrapper(IValueSchema<T> real)
            {
                _real = real;
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

            public IValueSerializer Serializer
            {
                get { return _real.Serializer; }
                set { _real.Serializer = value; }
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
        #endregion
    }
}