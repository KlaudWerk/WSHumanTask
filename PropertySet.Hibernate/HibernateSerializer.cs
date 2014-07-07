using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Klaudwerk.PropertySet.Serialization;

namespace Klaudwerk.PropertySet.Hibernate
{
    internal class HibernateSerializer
    {

        private static readonly Dictionary<Type, Action<HibernatePropertyElement,object, IValueSchema<object>>>
            _serializationActions = new Dictionary<Type, Action<HibernatePropertyElement, object, IValueSchema<object>>>
                                        {
                                            {typeof(bool?),(e,v,s)=>
                                                               {
                                                                   e.IntValue = ((bool?)v).Value ? 1 : 0;
                                                                   e.SerializationHint = SerializationTypeHint.Bool;
                                                               }},
                                            {typeof(int?),(e,v,s)=>
                                                              {
                                                                  e.IntValue = (int?) v;
                                                                  e.SerializationHint = SerializationTypeHint.Int;
                                                              }},
                                            {typeof(double?),(e,v,s)=>
                                                                 {
                                                                     e.DoubleValue = (double?) v;
                                                                     e.SerializationHint = SerializationTypeHint.Double;
                                                                 }},
                                            {typeof(long?),(e,v,s)=>
                                                               {
                                                                   e.LongValue = (long?) v;
                                                                   e.SerializationHint = SerializationTypeHint.Long;
                                                               }},
                                            {typeof(DateTime?),(e,v,s)=>
                                                                   {
                                                                       e.DateTimeValue = (DateTime?) v;
                                                                       e.SerializationHint =SerializationTypeHint.DateTime;
                                                                   }},
                                            {typeof(string),(e,v,s)=>
                                                                {
                                                                    e.StringValue = (string)v;
                                                                    e.SerializationHint = SerializationTypeHint.String;

                                                                }},
                                            {typeof(byte[]),(e,v,s)=>
                                                                {
                                                                    e.RawValue = (byte[]) v;
                                                                    e.SerializationHint =SerializationTypeHint.ByteArray;
                                                                }},
                                        };
        private readonly JsonPropertySerializer _serializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="HibernateSerializer"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        public HibernateSerializer(IPropertySchemaFactory factory)
        {
            _serializer = new JsonPropertySerializer(factory);
        }

        /// <summary>
        /// Serializes the element.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="schema">The schema.</param>
        /// <returns></returns>
        public HibernatePropertyElement SerializeElement(object value,IValueSchema<object> schema)
        {
            HibernatePropertyElement element = new HibernatePropertyElement();
            if(schema!=null)
            {
                string sBody;
                Type sType;
                _serializer.SerializeSchema(schema,out sType,out sBody);
                element.Schema=new SchemaElement{SchemaBody = sBody,SchemaType = sType.FullName};
            }
            if(value==null)
            {
                element.SerializationHint = SerializationTypeHint.Null;
            }
            else
            {
                
            }
            return element;
        }
    }
}
