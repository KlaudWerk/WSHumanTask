using System;
using System.IO;
using Newtonsoft.Json;

namespace HumanTask.ValueSet.Serialization
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
            JsonSerializer serializer=new JsonSerializer();
            object deserialized = serializer.Deserialize(new StringReader(jsonString), schemaType);
            ISchemaVisitable visitable = deserialized as ISchemaVisitable;
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
