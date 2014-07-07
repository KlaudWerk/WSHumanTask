using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace HumanTask.ValueSet
{

    /// <summary>
    /// Base Schema
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    [DataContract]
    public abstract class SchemaBase<T>:IValueSchema<T> 
    {
        private T _defaultValue;

        /// <summary>
        /// Gets the type of the property.
        /// </summary>
        /// <value>The type.</value>
        public Type Type { get { return typeof (T); } }

        /// <summary>
        /// Gets the default value.
        /// </summary>
        /// <value>The default value.</value>
        public virtual T DefaultValue { 
            get { return _defaultValue; } 
            set
            {
                Validate(value);
                _defaultValue = value;
                HasDefault = true;
            } 
        }

        /// <summary>
        /// Gets a value indicating whether this instance has default value.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has default; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool HasDefault { get; set; }

        /// <summary>
        /// Gets the list of possible values.
        /// </summary>
        /// <value>The possible values.</value>
        public abstract IEnumerable<T> PossibleValues { get; set; }

        /// <summary>
        /// Gets or sets the serializer that helps to serialize and deserialize this value.
        /// </summary>
        /// <value>The serializer.</value>
        public IValueSerializer Serializer { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is read only.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is read only; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool ReadOnly { get; set; }

        /// <summary>
        /// Gets a value indicating whether the NULL value allowed or not
        /// </summary>
        /// <value><c>true</c> if the null value allowed; otherwise, <c>false</c>.</value>
        [DataMember]
        public bool AllowNull { get; set; }

        /// <summary>
        /// Validates the specified value on set.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Validate(object value)
        {
            if (value == null)
            {
                if (!AllowNull)
                    throw new PropertyValidationException("null");
                return;
            }
            if(!typeof(T).IsAssignableFrom(value.GetType()))
                throw new PropertyValidationException("type");
            T val=OnValidate(value);
            if (PossibleValues != null && PossibleValues.Count() != 0 && PossibleValues.Count(v => v.Equals(val)) == 0)
                throw new PropertyValidationException("invalid value");
        }

        /// <summary>
        /// Perrform type-specific validation
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        protected abstract T OnValidate(object value);

        /// <summary>
        /// Converts the specified source to the schema's target type.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public abstract T Convert(object source);

        /// <summary>
        /// Accepts the schema visitor.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        public abstract void Accept(IValueSchemaVisitor visitor);


        /// <summary>
        /// Compare the base properties of a schema
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        protected bool SchemasEqual(SchemaBase<T> other)
        {
            if (other == null)
                return false;
            return ComparePossibleValues(other) && 
                AreValuesEqual(DefaultValue, other.DefaultValue) && 
                other.AllowNull == AllowNull &&
                   other.ReadOnly == ReadOnly;
        }

        /// <summary>
        /// Check if two values of T are equals
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        protected abstract bool AreValuesEqual(T value, T other);

        /// <summary>
        /// Compares the array of possible values.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        private bool ComparePossibleValues(IValueSchema<T> other)
        {
            if (PossibleValues == null && other.PossibleValues == null)
                return true;
            if (PossibleValues == null || other.PossibleValues == null)
            {
                return false;
            }
            if (PossibleValues.Count() != other.PossibleValues.Count())
                return false;
            for(int i=0; i<PossibleValues.Count(); i++)
            {
                if (!AreValuesEqual(PossibleValues.ElementAt(i), other.PossibleValues.ElementAt(i)))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Returns a hash code for possible values.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        protected int GetHashCode(IEnumerable<T> array)
        {
            unchecked
            {
                return array == null ? 0 : array.Aggregate(17, (current, element) => current*31 + element.GetHashCode());
            }
        }

    }
}