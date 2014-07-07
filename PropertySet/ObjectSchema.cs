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
using Klaudwerk.PropertySet.Serialization;

namespace Klaudwerk.PropertySet
{
    /// <summary>
    /// Simple object schema
    /// </summary>
    [Serializable]
    [DataContract]
    public class ObjectSchema:SchemaBase<object>
    {
        private readonly Type _requestedType;
        #region Overrides of SchemaBase<object>
        [DataMember]
        public override object DefaultValue
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
        public override IEnumerable<object> PossibleValues
        {
            get; set; 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectSchema"/> class.
        /// </summary>
        /// <param name="requestedType">Type of the requested.</param>
        public ObjectSchema(Type requestedType)
        {
            _requestedType = requestedType;
            Serializer=new DefaultValueSerializer(_requestedType);
        }

        /// <summary>
        /// Perrform type-specific validation
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        protected override object OnValidate(object value)
        {
            if(value==null)
                throw new PropertyValidationException("cast");
            return value;
        }

   
        /// <summary>
        /// Converts the specified source to the schema's target type.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public override object Convert(object source)
        {
            return source;
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
        /// Check if two values of T are equals
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        protected override bool AreValuesEqual(object value, object other)
        {
            return Equals(value, other);
        }
        #endregion

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
            ObjectSchema other = obj as ObjectSchema;
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
                int hashCode = 33 + GetHashCode(PossibleValues);
                return DefaultValue == null ? hashCode : hashCode ^ DefaultValue.GetHashCode();
            }
        }
    }
}
