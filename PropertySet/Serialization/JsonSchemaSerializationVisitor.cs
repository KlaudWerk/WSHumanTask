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
    /// JSON serialization visitor
    /// </summary>
    public class JsonSchemaSerializationVisitor:IValueSchemaVisitor
    {
        public Type SchemaType { get; private set; }
        public Type ValueType { get; private set; }
        public string JsonValue { get; private set; }

        /// <summary>
        /// Visits the specified schema.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="schema">The schema.</param>
        public void Visit<T>(IValueSchema<T> schema) where T : class
        {
            SchemaType = schema.GetType();
            ValueType = schema.Type;
            JsonValue = Serialize(schema);
        }

        /// <summary>
        /// Visits the specified schema.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="schema">The schema.</param>
        public void Visit<T>(IValueSchema<T?> schema) where T : struct
        {
            SchemaType = schema.GetType();
            ValueType = schema.Type;
            JsonValue = Serialize(schema);

        }

        /// <summary>
        /// Serializes the specified schema to JSON .
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <returns></returns>
        private static string Serialize(object schema)
        {
            JsonSerializer json=new JsonSerializer();
            StringWriter writer = new StringWriter();
            json.Serialize(writer,schema);
            writer.Flush();
            return writer.ToString();
        }

    }
}
