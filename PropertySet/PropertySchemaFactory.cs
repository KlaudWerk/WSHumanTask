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
using System.Reflection;
using Klaudwerk.PropertySet.Serialization;


namespace Klaudwerk.PropertySet
{
    /// <summary>
    /// Simple default Value Schema factory class
    /// </summary>
    public class PropertySchemaFactory:IPropertySchemaFactory
    {
        private static readonly Dictionary<Type,Func<IValueSchema<object>>> 
            _defaultSchemas=new Dictionary<Type, Func<IValueSchema<object>>>
         {
            {typeof(string),()=>new StringSchema().Wrap()},                                                                                  
            {typeof(int),()=>new IntSchema().Wrap()},
            {typeof(int?),()=>new IntSchema().Wrap()},
            {typeof(long),()=>new LongSchema().Wrap()},
            {typeof(long?),()=>new LongSchema().Wrap()},
            {typeof(double),()=>new DoubleSchema().Wrap()},
            {typeof(double?),()=>new DoubleSchema().Wrap()},
            {typeof(bool),()=>new BoolSchema().Wrap()},
            {typeof(bool?),()=>new BoolSchema().Wrap()},
            {typeof(DateTime),()=>new DateTimeSchema().Wrap()},
            {typeof(DateTime?),()=>new DateTimeSchema().Wrap()},
         };

        /// <summary>
        /// Register an Assembly Resolver function
        /// </summary>
        /// <value>The assembly resolver.</value>
        public Func<AssemblyName, Assembly> AssemblyResolver { get; set; }
        /// <summary>
        /// Gets or sets the type resolver.
        /// </summary>
        /// <value>The type resolver.</value>
        public Func<Assembly, string, bool, Type> TypeResolver { get; set; }

        /// <summary>
        /// Creates new schema for a property of type t
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns></returns>
        public IValueSchema<object> Create(Type t)
        {
            Func<IValueSchema<object>> schema;
            if (!_defaultSchemas.TryGetValue(t, out schema))
                return new ObjectSchema(t);
            return schema.Invoke();
        }

        /// <summary>
        /// Creates the specified type name.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <returns></returns>
        public IValueSchema<object> Create(string typeName)
        {
            return Create(ResolveType(typeName));
        }

        /// <summary>
        /// Registers the schema for this factory.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <param name="schema">The schema.</param>
        public void RegisterSchema(Type t, Func<IValueSchema<object>> schema)
        {
            _defaultSchemas[t] = schema;
        }

        /// <summary>
        /// Serializes the schema.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <param name="schemaType">Type of the schema.</param>
        /// <param name="schemaBody">The schema body.</param>
        public void SerializeSchema(IValueSchema<object> schema, out Type schemaType, out string schemaBody)
        {
            JsonSchemaSerializationVisitor visitor = new JsonSchemaSerializationVisitor();
            schema.Accept(visitor);
            schemaType = visitor.SchemaType;
            schemaBody = visitor.JsonValue;
        }

        /// <summary>
        /// Deserializes the schema.
        /// </summary>
        /// <param name="schemaBody">The schema body.</param>
        /// <param name="schemaTypeName">Name of the schema type.</param>
        /// <returns></returns>
        public IValueSchema<object> DeserializeSchema(string schemaBody, string schemaTypeName)
        {
            Type schemaType = ResolveType(schemaTypeName);

            return !string.IsNullOrEmpty(schemaBody) ?
                JsonSchemaDeserializer.Deserialize(schemaType, schemaBody) : Create(typeof(object)).Wrap();
        }
        /// <summary>
        /// Resolves the type.
        /// Tries to resolve the type;
        /// If resolve fails, try to resolve using assembly and types resolving functions
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        private Type ResolveType(string type)
        {
            Type resolved;
            try
            {
                resolved = Type.GetType(type, true, true);
            }
            catch (TypeLoadException)
            {
                resolved = Type.GetType(type, AssemblyResolver, TypeResolver, true, true);
            }
            return resolved;
        }
    }
}
