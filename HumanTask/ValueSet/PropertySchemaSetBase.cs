using System;
using System.Collections.Generic;

namespace HumanTask.ValueSet
{
    /// <summary>
    /// Base class for Property Schema set
    /// </summary>
    public abstract class PropertySchemaSetBase:IPropertySchemaSet
    {
        protected abstract IDictionary<string, IValueSchema<object>> SchemaStore { get; }

        protected IPropertySchemaFactory SchemaFactory { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertySchemaSetBase"/> class.
        /// </summary>
        /// <param name="schemaFactory">The schema factory.</param>
        protected PropertySchemaSetBase(IPropertySchemaFactory schemaFactory)
        {
            SchemaFactory = schemaFactory;
        }

        #region Implementation of IPropertySchemaSet

        public IEnumerable<KeyValuePair<string, IValueSchema<object>>> Schemas
        {
            get { return SchemaStore; }
        }

        /// <summary>
        /// Gets the type of the schema.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public Type GetSchemaType(string name)
        {
            IValueSchema<object> schema;
            if (!SchemaStore.TryGetValue(name, out schema))
                throw new KeyNotFoundException(name);
            return schema.Type;
        }

        /// <summary>
        /// Removes the schema.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public bool RemoveSchema(string name)
        {
            return SchemaStore.Remove(name);
        }

        /// <summary>
        /// Gets the schema.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public IValueSchema<object> GetSchema(string name)
        {
            IValueSchema<object> schema;
            if (!SchemaStore.TryGetValue(name, out schema))
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
            SchemaStore[name] = wrapped;
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
            if (!SchemaStore.TryGetValue(name, out objSchema))
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
            SchemaStore[name] = wrapped;
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
            if (!SchemaStore.TryGetValue(name, out objSchema))
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
        public void RemoveAll()
        {
            SchemaStore.Clear();
        }

        #endregion
    }
}
