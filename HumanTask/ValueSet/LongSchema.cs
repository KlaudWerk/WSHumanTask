using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace HumanTask.ValueSet
{
    [Serializable]
    [DataContract]
    public class LongSchema:SchemaBase<long?>
    {
        [DataMember]
        public override long? DefaultValue
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
        /// Gets or sets the max value.
        /// </summary>
        /// <value>The max value.</value>
        [DataMember]
        public long MaxValue { get; set; }
        /// <summary>
        /// Gets or sets the min value.
        /// </summary>
        /// <value>The min value.</value>
        [DataMember]
        public long MinValue { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LongSchema"/> class.
        /// </summary>
        public LongSchema()
        {
            MinValue = Int64.MinValue;
            MaxValue = Int64.MaxValue;
        }


        /// <summary>
        /// Gets the list of possible values.
        /// </summary>
        /// <value>The possible values.</value>
        [DataMember]
        public override IEnumerable<long?> PossibleValues { get; set; }

        protected override long? OnValidate(object value)
        {
            long? val = Convert(value);
            if(val<MinValue || val>MaxValue)
                throw new PropertyValidationException("minmax");
            return val;
        }

        /// <summary>
        /// Converts the specified source to the schema's target type.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public override long? Convert(object source)
        {
            return System.Convert.ToInt64(source);
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
        protected override bool AreValuesEqual(long? value, long? other)
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
            if (ReferenceEquals(this, obj))
                return true;
            LongSchema other = obj as LongSchema;
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

    }
}