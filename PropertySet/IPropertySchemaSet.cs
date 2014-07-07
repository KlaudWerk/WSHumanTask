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
    /// Properties schema set
    /// </summary>
    public interface IPropertySchemaSet
    {
        /// <summary>
        /// Gets the schema factory.
        /// </summary>
        IPropertySchemaFactory SchemaFactory { get; }
        /// <summary>
        /// Gets the schemas.
        /// </summary>
        IEnumerable<KeyValuePair<string, IValueSchema<object>>> Schemas { get; }
        /// <summary>
        /// Gets the type of the schema.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        Type GetSchemaType(string name);
        /// <summary>
        /// Removes the schema.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        bool RemoveSchema(string name);
        /// <summary>
        /// Gets the schema.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        IValueSchema<object> GetSchema(string name);
        /// <summary>
        /// Gets the default schema.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        IValueSchema<object> GetDefaultSchema(Type type);
        /// <summary>
        /// Sets the schema for a class property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <param name="schema">The schema.</param>
        void SetSchema<T>(string name, IValueSchema<T> schema) where T : class;
        /// <summary>
        /// Tries to get a schema for a property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <param name="schema">The schema.</param>
        /// <returns></returns>
        bool TryGetSchema<T>(string name, out IValueSchema<T> schema) where T : class;
        /// <summary>
        /// Sets the schema for a structure property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <param name="schema">The schema.</param>
        void SetSchema<T>(string name, IValueSchema<T?> schema) where T : struct;
        /// <summary>
        /// Tries to get a schema for struct property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <param name="schema">The schema.</param>
        /// <returns></returns>
        bool TryGetSchema<T>(string name, out IValueSchema<T?> schema) where T : struct;
        /// <summary>
        /// Removes all schemas.
        /// </summary>
        void RemoveAll();
    }
}