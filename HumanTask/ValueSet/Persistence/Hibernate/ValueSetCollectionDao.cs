using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HumanTask.ValueSet.Serialization;
using NHibernate;

namespace HumanTask.ValueSet.Persistence.Hibernate
{
    /// <summary>
    /// The DAO class for the Value Set collection
    /// </summary>
    internal class ValueSetCollectionDao
    {
        private static readonly Dictionary<Type,Action<PropertyElement,object>> 
            _setMap=new Dictionary<Type, Action<PropertyElement, object>>
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
        public ISessionFactory SessionFactory { get; set; }

        public void Store(ISession session, string name,object value,IValueSchema<object> schema)
        {
            PropertyElement element=new PropertyElement();
            Action<PropertyElement, object> action;
            if(_setMap.TryGetValue(schema.Type, out action))
            {
                action.Invoke(element,value);
            }
            else
            {
                if(schema.Serializer!=null)
                {
                    
                }
                else
                {
                    
                }
            }
        }
    }
}
