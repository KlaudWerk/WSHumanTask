using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using log4net;
using Newtonsoft.Json;

namespace HumanTask.ValueSet.Serialization
{
    /// <summary>
    /// Property Element serializer
    /// </summary>
    public class JsonPropertySerializer
    {
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly JsonSerializer _serializer=new JsonSerializer(); 
        private static readonly Dictionary<Type, Action<PropertyElement, object>>
                   _setMap = new Dictionary<Type, Action<PropertyElement, object>>
                        {
                            {typeof(int?),(e,o)=>
                                              {
                                                  e.ValInt = (int?) o;
                                                  e.SerializationValueType = SerializationValueType.Int;
                                              }},
                            {typeof(bool?),(e,o)=>
                                               {
                                                   e.ValInt = ((bool?) o).HasValue && ((bool?) o).Value ? 1 :0;
                                                   e.SerializationValueType = SerializationValueType.Bool;
                                               }},
                            {typeof(double?),(e,o)=>
                                                 {
                                                     e.ValDouble = (double?) o;
                                                     e.SerializationValueType = SerializationValueType.Double;
                                                 }},
                            {typeof(DateTime?),(e,o)=>
                                                   {
                                                       e.ValDateTime = (DateTime?) o;
                                                       e.SerializationValueType = SerializationValueType.DateTime;
                                                   }},
                            {typeof(long?),(e,o)=>
                                               {
                                                   e.ValLong = (long?) o;
                                                   e.SerializationValueType = SerializationValueType.Long;
                                               }},
                            {typeof(string),(e,o)=>
                                                {
                                                    e.ValString = (string) o;
                                                    e.SerializationValueType = SerializationValueType.String;
                                                }},
                            {typeof(byte[]),(e,o)=>
                                                {
                                                    e.ValRaw = (byte[]) o;
                                                    e.SerializationValueType = SerializationValueType.ByteArray;
                                                }}
                        };

        private static readonly Dictionary<SerializationValueType, Func<PropertyElement, object>>
            _getMap = new Dictionary<SerializationValueType, Func<PropertyElement, object>>
                          {
                              {SerializationValueType.Int, e =>{return e.ValInt;}},
                              {SerializationValueType.Bool, e =>{return e.ValInt == 1;}},
                              {SerializationValueType.Double, e =>{return e.ValDouble;}},
                              {SerializationValueType.DateTime, e =>{return e.ValDateTime;}},
                              {SerializationValueType.Long, e =>{return e.ValLong;}},
                              {SerializationValueType.String, e =>{return e.ValString;}},
                              {SerializationValueType.ByteArray, e =>{return e.ValRaw;}}
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
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <param name="schema">The schema.</param>
        /// <returns></returns>
        public  PropertyElement Serialize(string name,object value,IValueSchema<object> schema)
        {
            PropertyElement element=new PropertyElement();
            JsonSchemaSerializationVisitor visitor=new JsonSchemaSerializationVisitor();
            schema.Accept(visitor);
            Action<PropertyElement, object> action;
            if(_setMap.TryGetValue(visitor.ValueType,out action))
            {
                action.Invoke(element,value);
            }
            else
            {
                StringWriter writer=new StringWriter();
                _serializer.Serialize(writer,value);
                element.ValString = writer.ToString();
            }
            element.SchemaType = visitor.SchemaType.FullName;
            element.TypeOfValue = visitor.ValueType.FullName;
            element.SchemaBody = visitor.JsonValue;
            return element;
        }

        /// <summary>
        /// Deserializes the specified element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="schema">The schema.</param>
        /// <returns></returns>
        public object Deserialize(PropertyElement element,out IValueSchema<object> schema)
        {
            object value;
            Type schemaType = ResolveType(element.SchemaType);
            Type valueType = ResolveType(element.TypeOfValue);

            schema = !string.IsNullOrEmpty(element.SchemaBody) ? 
                JsonSchemaDeserializer.Deserialize(schemaType, element.SchemaBody) : 
                _schemaFactory.Create(valueType).Wrap();

            Func<PropertyElement, object> func;
            if (_getMap.TryGetValue(element.SerializationValueType, out func))
            {
                value = func.Invoke(element);
            }
            else
            {
                StringReader reader=new StringReader(element.ValString);
                value = _serializer.Deserialize(reader, valueType);
            }
            return value;
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
