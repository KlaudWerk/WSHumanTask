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
using System.IO;
using Newtonsoft.Json;

namespace Klaudwerk.PropertySet.Serialization
{
    /// <summary>
    /// Json Deserializer class for Property Schemas
    /// </summary>
    public class JsonSchemaDeserializer
    {
        /// <summary>
        /// Deserializes the specified schema type.
        /// Json deserialization
        /// </summary>
        /// <param name="schemaType">Type of the schema.</param>
        /// <param name="jsonString">The json string.</param>
        /// <returns></returns>
        public static IValueSchema<object> Deserialize(Type schemaType,string jsonString)
        {
            SchemaDeserializeWrapVisitor visitor=new SchemaDeserializeWrapVisitor();
            ISchemaVisitable visitable;
            if (string.IsNullOrEmpty(jsonString))
            {
                object obj = Activator.CreateInstance(schemaType);
                visitable = obj as ISchemaVisitable;
            }
            else
            {
                JsonSerializer serializer = new JsonSerializer();
                object deserialized = serializer.Deserialize(new StringReader(jsonString), schemaType);
                visitable = deserialized as ISchemaVisitable;
            }
            if(visitable==null)
                throw new ArgumentException(string.Format("Object of type {0} does not implement {1} interface.",
                    schemaType.FullName,typeof(ISchemaVisitable).FullName));
            visitable.Accept(visitor);
            return visitor.Schema;
        }

        /// <summary>
        /// Vistor Wrapper class
        /// </summary>
        private class SchemaDeserializeWrapVisitor : IValueSchemaVisitor
        {
            /// <summary>
            /// Gets or sets the schema.
            /// </summary>
            /// <value>
            /// The schema.
            /// </value>
            public IValueSchema<object> Schema { get; private set; }

            /// <summary>
            /// Wrap a reference schema
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="schema">The schema.</param>
            public void Visit<T>(IValueSchema<T> schema) where T : class
            {
                Schema = schema.Wrap();
            }

            /// <summary>
            /// Wrap a valut type schema
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="schema">The schema.</param>
            public void Visit<T>(IValueSchema<T?> schema) where T : struct
            {
                Schema = schema.Wrap();
            }
        }

    }
}
