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

namespace Klaudwerk.PropertySet
{
    /// <summary>
    /// Schema Factory interface
    /// </summary>
    public interface IPropertySchemaFactory
    {
        /// <summary>
        /// Creates new schema for a property of type t
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns></returns>
        IValueSchema<object> Create(Type t);
        /// <summary>
        /// Creates the specified type name.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <returns></returns>
        IValueSchema<object> Create(string typeName);
        /// <summary>
        /// Registers the schema for this factory.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <param name="schema">The schema.</param>
        void RegisterSchema(Type t, Func<IValueSchema<object>> schema);
        /// <summary>
        /// Serializes the schema.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <param name="schemaType">Type of the schema.</param>
        /// <param name="schemaBody">The schema body.</param>
        void SerializeSchema(IValueSchema<object> schema, out Type schemaType, out string schemaBody );
        /// <summary>
        /// Deserializes the schema.
        /// </summary>
        /// <param name="schemaBody">The schema body.</param>
        /// <param name="schemaTypeName">Name of the schema type.</param>
        /// <returns></returns>
        IValueSchema<object> DeserializeSchema(string schemaBody, string schemaTypeName);
    }
}