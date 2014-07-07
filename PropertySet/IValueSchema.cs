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

namespace Klaudwerk.PropertySet
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
        IValueSerializer<T> Serializer { get; set; }
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