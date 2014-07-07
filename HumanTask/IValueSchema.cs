using System;
using System.Collections.Generic;

namespace HumanTask
{
    /// <summary>
    /// The Value Schema
    /// Provides the way to validate the Value set operation.
    /// Can contain a default value 
    /// </summary>
    public interface IValueSchema<T>:ISchemaVisitable
    {
        /// <summary>
        /// Gets the type of the property.
        /// </summary>
        /// <value>The type.</value>
        Type Type { get; }
        /// <summary>
        /// Gets the default value.
        /// </summary>
        /// <value>The default value.</value>
        T DefaultValue { get; set; }
        /// <summary>
        /// Gets a value indicating whether this instance has default value.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has default; otherwise, <c>false</c>.
        /// </value>
        bool HasDefault { get; set; }
        /// <summary>
        /// Gets the list of possible values.
        /// </summary>
        /// <value>The possible values.</value>
        IEnumerable<T> PossibleValues { get; set; }
        /// <summary>
        /// Gets or sets the serializer that helps to serialize and deserialize this value.
        /// </summary>
        /// <value>The serializer.</value>
        IValueSerializer Serializer { get; set; }
        /// <summary>
        /// Gets a value indicating whether this instance is read only.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is read only; otherwise, <c>false</c>.
        /// </value>
        bool ReadOnly { get; set; }
        /// <summary>
        /// Gets a value indicating whether the NULL value allowed or not
        /// </summary>
        /// <value><c>true</c> if the null value allowed; otherwise, <c>false</c>.</value>
        bool AllowNull { get; set; }
        /// <summary>
        /// Validates the specified value on set.
        /// </summary>
        /// <param name="value">The value.</param>
        void Validate(object value);

        /// <summary>
        /// Converts the specified source to the schema's target type.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        T Convert(object source);

    }
}