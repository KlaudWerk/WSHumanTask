using System;
using System.Collections.Generic;
using Klaudwerk.PropertySet.Serialization;
using NHibernate;

namespace Klaudwerk.PropertySet.Hibernate
{
    /// <summary>
    /// The DAO class for the Value Set collection
    /// </summary>
    internal class ValueSetCollectionDao
    {
        private static readonly Dictionary<Type, Action<PropertyElement, object>>
            _setMap = new Dictionary<Type, Action<PropertyElement, object>>();
        //                {
        //                    {typeof(int?),(e,o)=>
        //                                      {
        //                                          e.ValInt = (int?) o;
        //                                          e.SerializationHint = SerializationHint.Int;
        //                                      }},
        //                    {typeof(bool?),(e,o)=>
        //                                       {
        //                                           e.ValInt = ((bool?) o).HasValue && ((bool?) o).Value ? 1 :0;
        //                                           e.SerializationHint = SerializationHint.Bool;
        //                                       }},
        //                    {typeof(double?),(e,o)=>
        //                                         {
        //                                             e.ValDouble = (double?) o;
        //                                             e.SerializationHint = SerializationHint.Double;
        //                                         }},
        //                    {typeof(DateTime?),(e,o)=>
        //                                           {
        //                                               e.ValDateTime = (DateTime?) o;
        //                                               e.SerializationHint = SerializationHint.DateTime;
        //                                           }},
        //                    {typeof(long?),(e,o)=>
        //                                       {
        //                                           e.ValLong = (long?) o;
        //                                           e.SerializationHint = SerializationHint.Long;
        //                                       }},
        //                    {typeof(string),(e,o)=>
        //                                        {
        //                                            e.ValString = (string) o;
        //                                            e.SerializationHint = SerializationHint.String;
        //                                        }},
        //                    {typeof(byte[]),(e,o)=>
        //                                        {
        //                                            e.ValRaw = (byte[]) o;
        //                                            e.SerializationHint = SerializationHint.ByteArray;
        //                                        }}


        //                }; 
        public ISessionFactory SessionFactory { get; set; }

        public void Store(ISession session, HibernatePropertyElement element)
        {
        }
    }
}
