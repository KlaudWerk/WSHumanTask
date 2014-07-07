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
    /// Base class for Property Schema set
    /// </summary>
    public abstract class PropertySchemaSetBase:IPropertySchemaSet
    {
        /// <summary>
        /// Gets the schema factory.
        /// </summary>
        public IPropertySchemaFactory SchemaFactory { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertySchemaSetBase"/> class.
        /// </summary>
        /// <param name="schemaFactory">The schema factory.</param>
        protected PropertySchemaSetBase(IPropertySchemaFactory schemaFactory)
        {
            SchemaFactory = schemaFactory;
        }

        #region Implementation of IPropertySchemaSet

        public abstract IEnumerable<KeyValuePair<string, IValueSchema<object>>> Schemas { get; }

        /// <summary>
        /// Gets the type of the schema.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public Type GetSchemaType(string name)
        {
            IValueSchema<object> schema;
            if (!OnTryGetValue(name, out schema))
                throw new KeyNotFoundException(name);
            return schema.Type;
        }

        /// <summary>
        /// Removes the schema.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public abstract bool RemoveSchema(string name);

        /// <summary>
        /// Gets the schema.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public IValueSchema<object> GetSchema(string name)
        {
            IValueSchema<object> schema;
            if (!OnTryGetValue(name, out schema))
                throw new KeyNotFoundException(name);
            return schema;
        }

        /// <summary>
        /// Gets the default schema.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public IValueSchema<object> GetDefaultSchema(Type type)
        {
            return SchemaFactory.Create(type);
        }

        /// <summary>
        /// Sets the schema for a class property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <param name="schema">The schema.</param>
        public void SetSchema<T>(string name, IValueSchema<T> schema) where T : class
        {
            IValueSchema<object> wrapped = schema.Wrap();
            OnSetSchema(name, wrapped);
        }

        /// <summary>
        /// Tries to get a schema for a property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <param name="schema">The schema.</param>
        /// <returns></returns>
        public bool TryGetSchema<T>(string name, out IValueSchema<T> schema) where T : class
        {
            IValueSchema<object> objSchema;
            if (!OnTryGetValue(name, out objSchema))
            {
                schema = null;
                return false;
            }
            schema = objSchema.UnwrapRefType<T>();
            return true;
        }
        

        /// <summary>
        /// Sets the schema for a structure property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <param name="schema">The schema.</param>
        public void SetSchema<T>(string name, IValueSchema<T?> schema) where T : struct
        {
            IValueSchema<object> wrapped = schema.Wrap();
            OnSetSchema(name, wrapped);
        }

        /// <summary>
        /// Tries to get a schema for struct property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <param name="schema">The schema.</param>
        /// <returns></returns>
        public bool TryGetSchema<T>(string name, out IValueSchema<T?> schema) where T : struct
        {
            IValueSchema<object> objSchema;
            if (!OnTryGetValue(name, out objSchema))
            {
                schema = null;
                return false;
            }
            schema = objSchema.UnwrapValueType<T>();
            return true;
        }

        /// <summary>
        /// Removes all schemas.
        /// </summary>
        public abstract void RemoveAll();

        #endregion

        /// <summary>
        /// Create or save the schema
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="wrapped">The wrapped.</param>
        protected abstract void OnSetSchema(string name, IValueSchema<object> wrapped);

        /// <summary>
        /// Try to get the schema
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="objSchema">The obj schema.</param>
        /// <returns></returns>
        protected abstract bool OnTryGetValue(string name, out IValueSchema<object> objSchema);
    }
}
