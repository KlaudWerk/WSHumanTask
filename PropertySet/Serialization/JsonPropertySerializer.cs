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
using System.IO;
using System.Reflection;
using log4net;
using Newtonsoft.Json;

namespace Klaudwerk.PropertySet.Serialization
{
    /// <summary>
    /// Property Element serializer
    /// </summary>
    public class JsonPropertySerializer
    {
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly JsonSerializer _serializer=new JsonSerializer(); 
        private static readonly Dictionary<Type, Action<PropertyValue, object>>
                   _setMap = new Dictionary<Type, Action<PropertyValue, object>>
                        {
                            {typeof(int?),(e,o)=>
                                              {
                                                  e.Value = (int?) o;
                                                  e.SerializationHint = SerializationTypeHint.Int;
                                              }},
                            {typeof(bool?),(e,o)=>
                                               {
                                                   e.Value = ((bool?) o).HasValue && ((bool?) o).Value ? 1 :0;
                                                   e.SerializationHint = SerializationTypeHint.Bool;
                                               }},
                            {typeof(double?),(e,o)=>
                                                 {
                                                     e.Value = (double?)o;
                                                     e.SerializationHint = SerializationTypeHint.Double;
                                                 }},
                            {typeof(DateTime?),(e,o)=>
                                                   {
                                                       e.Value = (DateTime?)o;
                                                       e.SerializationHint = SerializationTypeHint.DateTime;
                                                   }},
                            {typeof(long?),(e,o)=>
                                               {
                                                   e.Value = (long?)o;
                                                   e.SerializationHint = SerializationTypeHint.Long;
                                               }},
                            {typeof(string),(e,o)=>
                                                {
                                                    e.Value = o;
                                                    e.SerializationHint = SerializationTypeHint.String;
                                                }},
                            {typeof(byte[]),(e,o)=>
                                                {
                                                    e.Value = o;
                                                    e.SerializationHint = SerializationTypeHint.ByteArray;
                                                }}
                        };

        private static readonly Dictionary<SerializationTypeHint, Func<PropertyElement, object>>
            _getMap = new Dictionary<SerializationTypeHint, Func<PropertyElement, object>>
                          {
                              {SerializationTypeHint.Int, e =>{return (int?)e.Value;}},
                              {SerializationTypeHint.Bool, e =>{return (int?)e.Value == 1;}},
                              {SerializationTypeHint.Double, e =>{return (double?)e.Value;}},
                              {SerializationTypeHint.DateTime, e =>{return (DateTime?)e.Value;}},
                              {SerializationTypeHint.Long, e =>{return (long?)e.Value;}},
                              {SerializationTypeHint.String, e =>{return (string)e.Value;}},
                              {SerializationTypeHint.ByteArray, e =>{return (byte[])e.Value;}}
                          };
        private readonly IPropertySchemaFactory _schemaFactory;

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
        /// Initializes a new instance of the <see cref="JsonPropertySerializer"/> class.
        /// </summary>
        /// <param name="schemaFactory">The schema factory.</param>
        public JsonPropertySerializer(IPropertySchemaFactory schemaFactory)
        {
            _schemaFactory = schemaFactory;
            AssemblyResolver = n =>
                                   {
                                       _logger.Error(string.Format("Assembly resolver: Cannot resolve the assembly:{0}",n.FullName));
                                       
                                       return null;
                                   };
            TypeResolver = (a, n, b) =>
                               {
                                   _logger.Error(string.Format("Type resolver: Cannot resolve the type {0} in assembly {1}",n,a.FullName));
                                   return null;
                               };
        }

        /// <summary>
        /// Serializes the property.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="schema">The schema.</param>
        /// <returns></returns>
        public  PropertyElement Serialize(object value,IValueSchema<object> schema)
        {
            PropertyElement element = new PropertyElement();
            if (schema != null)
            {
                Type stype;
                string sbody;
                SerializeSchema(schema,out stype,out sbody);
                element.Schema=new SchemaElement{SchemaBody = sbody,SchemaType = stype.FullName};
            }
            PropertyValue v = SerializeValue(value, schema);
            element.SerializationHint = v.SerializationHint;
            element.ValueType = v.ValueType;
            element.Value = v.Value;
            return element;
        }

        /// <summary>
        /// Serializes the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public PropertyValue SerializeValue(object value)
        {
            return SerializeValue(value, null);
        }
        /// <summary>
        /// Serializes the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="schema">The schema.</param>
        /// <returns></returns>
        public PropertyValue SerializeValue(object value,IValueSchema<object> schema)
        {
            PropertyValue element=new PropertyValue();
            if(value==null)
            {
                element.SerializationHint = SerializationTypeHint.Object;
                element.Value = null;
                return element;
            }
            Action<PropertyValue, object> action;
            Type type = schema == null ? value.GetType() : schema.Type;
            if (_setMap.TryGetValue(type, out action))
            {
                action.Invoke(element, value);
            }
            else
            {
                StringWriter writer = new StringWriter();
                _serializer.Serialize(writer, value);
                element.Value = writer.ToString();
                element.SerializationHint = SerializationTypeHint.Object;
                element.ValueType = type.FullName;
            }
            return element;
        }
        /// <summary>
        /// Serializes the schema.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <param name="schemaType">Type of the schema.</param>
        /// <param name="schemaJson">The schema json.</param>
        public void SerializeSchema(IValueSchema<object> schema,out Type schemaType,out string schemaJson)
        {
            JsonSchemaSerializationVisitor visitor = new JsonSchemaSerializationVisitor();
            schema.Accept(visitor);
            schemaType = visitor.SchemaType;
            schemaJson = visitor.JsonValue;
        }
        /// <summary>
        /// Deserializes the specified element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="schema">The schema.</param>
        /// <returns></returns>
        public object Deserialize(PropertyElement element,IValueSchema<object> schema)
        {
            object value;
            if (element.Value == null)
                return null;
            Func<PropertyElement, object> func;
            if (_getMap.TryGetValue(element.SerializationHint, out func))
            {
                value = func.Invoke(element);
            }
            else
            {
                StringReader reader=new StringReader((string)element.Value);
                value = _serializer.Deserialize(reader, schema.Type);
            }
            return value;
        }

        public object Deserialize(PropertyElement element)
        {
            object value;
            if (element.Value == null)
                return null;
            Func<PropertyElement, object> func;
            if (_getMap.TryGetValue(element.SerializationHint, out func))
            {
                value = func.Invoke(element);
            }
            else
            {
                StringReader reader = new StringReader((string)element.Value);
                value = _serializer.Deserialize(reader, ResolveType(element.ValueType));
            }
            return value;
        }

        /// <summary>
        /// Deserializes the schema.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public IValueSchema<object> DeserializeSchema(SchemaElement element)
        {
            Type schemaType = ResolveType(element.SchemaType);
 
            return !string.IsNullOrEmpty(element.SchemaBody) ?
                JsonSchemaDeserializer.Deserialize(schemaType, element.SchemaBody) :
                _schemaFactory.Create(typeof(object)).Wrap();
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
            catch(TypeLoadException)
            {
                resolved = Type.GetType(type, AssemblyResolver, TypeResolver, true, true);
            }
            return resolved;
        }
    }
}
