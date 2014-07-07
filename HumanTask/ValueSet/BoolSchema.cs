using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace HumanTask.ValueSet
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
    }
}