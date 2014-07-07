using System;
using System.Collections.Generic;

namespace HumanTask
{
    /// <summary>
    /// Properties schema set
    /// </summary>
    public interface IPropertySchemaSet
    {
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