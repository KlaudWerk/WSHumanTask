using System;
using System.IO;
using Newtonsoft.Json;

namespace HumanTask.ValueSet.Serialization
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
