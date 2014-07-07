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
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using Klaudwerk.PropertySet.Serialization;
using log4net;
using NHibernate;
using NHibernate.Context;

namespace Klaudwerk.PropertySet.Hibernate
{
    /// <summary>
    /// Value Set collection with NHibernate persistence
    /// </summary>
    public class HibernatePropertySetCollection : PropertySetCollectionBase
    {
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly Dictionary<SerializationTypeHint, Func<HibernatePropertySetCollection, HibernatePropertyElement, object>>
            _serializationMap = new Dictionary
                <SerializationTypeHint, Func<HibernatePropertySetCollection, HibernatePropertyElement, object>>
                                    {
                                        {SerializationTypeHint.Null,(c,e)=>null},
                                        {SerializationTypeHint.Int,(c,e)=>e.IntValue},
                                        {SerializationTypeHint.Long,(c,e)=>e.LongValue},
                                        {SerializationTypeHint.Bool,(c,e)=>e.IntValue!=null && e.IntValue==1},
                                        {SerializationTypeHint.Double,(c,e)=>e.DoubleValue},
                                        {SerializationTypeHint.DateTime,(c,e)=>e.DateTimeValue},
                                        {SerializationTypeHint.String,(c,e)=>e.StringValue},
                                        {SerializationTypeHint.ByteArray,(c,e)=>e.RawValue},
                                        {SerializationTypeHint.JsonString,(c,e)=>c.DeserializeJson(e)},
                                        {SerializationTypeHint.BinaryObject,(c,e)=>c.DeserializeBinary(e)}
                                    };
        private  readonly Dictionary<SerializationTypeHint,Func<HibernatePropertySetCollection,IValueSchema<object>>> 
            _hintSchemaMap=new Dictionary<SerializationTypeHint, Func<HibernatePropertySetCollection,IValueSchema<object>>>
                               {
                                        {SerializationTypeHint.Null,c=>null},
                                        {SerializationTypeHint.Int,c=>c.Schemas.SchemaFactory.Create(typeof(int?))},
                                        {SerializationTypeHint.Long,c=>c.Schemas.SchemaFactory.Create(typeof(long?))},
                                        {SerializationTypeHint.Bool,c=>c.Schemas.SchemaFactory.Create(typeof(bool?))},
                                        {SerializationTypeHint.Double,c=>c.Schemas.SchemaFactory.Create(typeof(double?))},
                                        {SerializationTypeHint.DateTime,c=>c.Schemas.SchemaFactory.Create(typeof(DateTime?))},
                                        {SerializationTypeHint.String,c=>c.Schemas.SchemaFactory.Create(typeof(string))},
                                        {SerializationTypeHint.ByteArray,c=>c.Schemas.SchemaFactory.Create(typeof(byte[]))},
                                        {SerializationTypeHint.JsonString,c=>{throw new ArgumentException("Schema must be provided");}},
                                        {SerializationTypeHint.BinaryObject,c=>{throw new ArgumentException("Schema must be provided");}}
                                   
                               };
        private readonly PropertySchemaSet _schemaSet;
        private readonly ISessionFactory _sessionFactory;
        private bool _isReadOnly = false;

        /// <summary>
        /// Gets or sets the unique collection id.
        /// </summary>
        /// <value>The id.</value>
        public Guid Id { get; private set; }

        /// <summary>
        /// Gets the Properties Schema Set.
        /// </summary>
        /// <value>The schemas.</value>
        public override IPropertySchemaSet Schemas
        {
            get { return _schemaSet; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HibernatePropertySetCollection"/> class.
        /// </summary>
        /// <param name="schemaFactory">The schema factory.</param>
        /// <param name="sessionFactory">The session factory.</param>
        /// <param name="collectionId">The collection id.</param>
        public HibernatePropertySetCollection(
            IPropertySchemaFactory schemaFactory,
            ISessionFactory sessionFactory,
            Guid collectionId)
        {
            Id = collectionId;
            _schemaSet = new PropertySchemaSet(this, schemaFactory);
            _sessionFactory = sessionFactory;
        }


        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public override IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return RunInSession<IEnumerator<KeyValuePair<string, object>>>(s =>
                             {
                                 PropertyElementsCollection collection = GetCollection(s);
                                 return
                                     collection.Elements.Select(
                                         e => new KeyValuePair<string, object>(e.Key, DeserializeElement(e.Value))).
                                         ToList().GetEnumerator();
                             }, (s, e) => { });
        }

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
        public override void Add(KeyValuePair<string, object> item)
        {
            Add(item.Key,item.Value);
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only. </exception>
        public override void Clear()
        {
            RunInSession(s =>
                             {
                                 StorageCollectionDao dao=new StorageCollectionDao(s);
                                 PropertyElementsCollection collection = dao.GetById(Id);
                                 if (collection == null)
                                     return false;
                                 collection.Elements.Clear();
                                 dao.Store(collection);
                                 return true;
                             }, (s, e) => { });
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
        /// </summary>
        /// <returns>
        /// true if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false.
        /// </returns>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        public override bool Contains(KeyValuePair<string, object> item)
        {
            return RunInSession(s=>
                                    {
                                        object val;
                                        return TryGetValue(item.Key, out val) ? Equals(item.Value, val): false ;
                                    },(s,e)=>{});
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param><param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param><exception cref="T:System.ArgumentNullException"><paramref name="array"/> is null.</exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than 0.</exception><exception cref="T:System.ArgumentException"><paramref name="array"/> is multidimensional.-or-The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"/> is greater than the available space from <paramref name="arrayIndex"/> 
        /// to the end of the destination <paramref name="array"/>.-or-Type <paramref name="T"/> cannot be cast automatically to the type of the destination <paramref name="array"/>.</exception>
        public override void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            RunInSession(s =>
                             {
                                 PropertyElementsCollection collection = GetCollection(s);
                                 collection.Elements.
                                     Select(e=>new KeyValuePair<string,object>(e.Key,DeserializeElement(e.Value))).
                                     ToList().CopyTo(array,arrayIndex);
                                 return true;
                             }, (s, e) => { });
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <returns>
        /// true if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
        public override bool Remove(KeyValuePair<string, object> item)
        {
            return Remove(item.Key);
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <returns>
        /// The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        public override int Count
        {
            get
            {
                return RunInSession(s => GetCollection(s).Elements.Count, (s, e) => { });
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only; otherwise, false.
        /// </returns>
        public override bool IsReadOnly { get { return _isReadOnly; } }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an element with the specified key.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an element with the key; otherwise, false.
        /// </returns>
        /// <param name="key">The key to locate in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.</param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception>
        public override bool ContainsKey(string key)
        {
            return RunInSession(
                s =>
                    {
                       PropertyElementsCollection collection = GetCollection(s);
                       return collection==null?false:collection.Elements.ContainsKey(key);
                    }, (s, e) => { });
        }

        /// <summary>
        /// Adds an element with the provided key and value to the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <param name="key">The object to use as the key of the element to add.</param><param name="value">The object to use as the value of the element to add.</param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception><exception cref="T:System.ArgumentException">An element with the same key already exists in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.</exception><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IDictionary`2"/> is read-only.</exception>
        public override void Add(string key, object value)
        {
            RunInSession(s =>
                             {
                                 AddValue(key, value);
                                 return true;
                             }, (s, e) => { });
        }

        /// <summary>
        /// Removes the element with the specified key from the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <returns>
        /// true if the element is successfully removed; otherwise, false.  This method also returns false if <paramref name="key"/> was not found in the original <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </returns>
        /// <param name="key">The key of the element to remove.</param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IDictionary`2"/> is read-only.</exception>
        public override bool Remove(string key)
        {
            return RunInSession(s =>{
                                            StorageCollectionDao dao = new StorageCollectionDao(s);
                                            PropertyElementsCollection collection= dao.GetById(Id);
                                            if (collection == null)
                                                return false;
                                            if(collection.Elements.Remove(key))
                                            {
                                                dao.Store(collection);
                                                return true;
                                            }
                                              return false;
            }, (s, e) => { });
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <returns>
        /// true if the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an element with the specified key; otherwise, false.
        /// </returns>
        /// <param name="key">The key whose value to get.</param><param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value"/> parameter. This parameter is passed uninitialized.</param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception>
        public override bool TryGetValue(string key, out object value)
        {
            value = RunInSession(s =>
                                     {
                                         HibernatePropertyElement v;
                                         return GetCollection(s).Elements.TryGetValue(key, out v)?
                                             DeserializeElement(v):null;
                                     }, (s, e) => { });
            return value != null;
        }

        /// <summary>
        /// Gets or sets the element with the specified key.
        /// </summary>
        /// <returns>
        /// The element with the specified key.
        /// </returns>
        /// <param name="key">The key of the element to get or set.</param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception><exception cref="T:System.Collections.Generic.KeyNotFoundException">The property is retrieved and <paramref name="key"/> is not found.</exception><exception cref="T:System.NotSupportedException">The property is set and the <see cref="T:System.Collections.Generic.IDictionary`2"/> is read-only.</exception>
        public override object this[string key]
        {
            get
            {
                HibernatePropertyElement element = RunInSession(s =>
                                 {
                                     StorageCollectionDao dao = new StorageCollectionDao(s);
                                     PropertyElementsCollection collection = dao.GetById(Id);
                                     return collection == null?null: collection.Elements[key];
                                 },
                    (s, e) => {});
                if (element == null)
                    throw new KeyNotFoundException(key);
                return DeserializeElement(element);
            }
            set
            {
                RunInSession(s =>
                                 {
                                     IValueSchema<object> schema;
                                     StorageCollectionDao dao = new StorageCollectionDao(s);
                                     PropertyElementsCollection collection = dao.GetCreateById(Id);
                                     HibernatePropertyElement element;
                                     if (collection.Elements.TryGetValue(key, out element))
                                     {
                                         schema = GetValueSchema(element);
                                     }
                                     else
                                     {
                                         element = new HibernatePropertyElement();
                                         schema =value==null?null: _schemaSet.SchemaFactory.Create(value.GetType());
                                     }
                                     if(schema!=null)
                                         schema.Validate(value);
                                     if (value == null)
                                     {
                                         element.SerializationHint = SerializationTypeHint.Null;
                                     }
                                     else
                                     {
                                         if(schema==null)
                                         {
                                             schema = Schemas.SchemaFactory.Create(value.GetType());
                                         }
                                         schema.Serializer.Serialize(value, element);
                                     }
                                     collection.Elements[key] = element;
                                     dao.Store(collection);
                                     return true;
                                 },
                             (s, e) => { });
            }
        }
        /// <summary>
        /// Saves the element to the collection
        /// </summary>
        private void SaveElement(string key,HibernatePropertyElement element)
        {
            RunInSession(s=>
                             {
                                 StorageCollectionDao dao=new StorageCollectionDao(s);
                                 PropertyElementsCollection collection = dao.GetCreateById(Id);
                                 collection.Elements[key] = element;
                                 dao.Store(collection);
                                 return 0;
                             },(s,e)=>{});                
        }

        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1"/> containing the keys of the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.Generic.ICollection`1"/> containing the keys of the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </returns>
        public override ICollection<string> Keys
        {
            get
            {
                return RunInSession<ICollection<string>>(s =>
                {
                    StorageCollectionDao dao=new StorageCollectionDao(s);
                    PropertyElementsCollection collection = dao.GetCreateById(Id);
                    return collection.Elements.Select(e => e.Key).ToList();

                }, (s, e) => { });
            }
        }

        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1"/> containing the values in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.Generic.ICollection`1"/> containing the values in the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </returns>
        public override ICollection<object> Values
        {
            get
            {
                return RunInSession<ICollection<object>>(s =>
                {
                    StorageCollectionDao dao = new StorageCollectionDao(s);
                    PropertyElementsCollection collection = dao.GetCreateById(Id);
                    return collection.Elements.Select(e =>DeserializeElement(e.Value)).ToList();

                }, (s, e) => { });
            }
        }

        /// <summary>
        /// Adds the with schema.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <param name="schema">The schema.</param>
        protected override void AddWithSchema(string name, object value, IValueSchema<object> schema)
        {
            schema.Validate(value);
            HibernatePropertyElement element = new HibernatePropertyElement();
            string schemaBody;
            Type schemaType;
            _schemaSet.SchemaFactory.SerializeSchema(schema, out schemaType, out schemaBody);
            element.Schema = new SchemaElement { SchemaBody = schemaBody, SchemaType = schemaType.FullName };
            schema.Serializer.Serialize(value, element);
            SaveElement(name,element);
        }
        /// <summary>
        /// Adds the value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        protected override void AddValue(string name, object value)
        {
            HibernatePropertyElement element = new HibernatePropertyElement();
            if (value == null)
            {
                element.SerializationHint = SerializationTypeHint.Null;
                SaveElement(name,element);
                return;
            }
            IValueSchema<object> schema = _schemaSet.SchemaFactory.Create(value.GetType());
            schema.Validate(value);
            schema.Serializer.Serialize(value, element);
            SaveElement(name,element);
        }

        #region Private Methods
        /// <summary>
        /// Deserializes the binary.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        private object DeserializeBinary(HibernatePropertyElement element)
        {
            IValueSchema<object> schema = GetValueSchema(element);
            return schema.Serializer.Deserialize(element.RawValue);
        }

        /// <summary>
        /// Deserializes the json.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        private object DeserializeJson(HibernatePropertyElement element)
        {
            IValueSchema<object> schema = GetValueSchema(element);
            return schema.Serializer.Deserialize(element.StringValue);
        }

        /// <summary>
        /// Gets the value schema.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        private IValueSchema<object> GetValueSchema(HibernatePropertyElement element)
        {
            IValueSchema<object> schema;
            if (element.Schema == null || element.Schema.SchemaBody == null)
            {
                
                schema = string.IsNullOrEmpty(element.ValueType)?
                    GetSchemaForHint(element.SerializationHint):
                    _schemaSet.SchemaFactory.Create(element.ValueType);
            }
            else
            {
                schema = _schemaSet.SchemaFactory.DeserializeSchema(element.Schema.SchemaBody, element.Schema.SchemaType);
            }
            return schema;
        }

        /// <summary>
        /// Gets the schema for hint.
        /// </summary>
        /// <param name="serializationHint">The serialization hint.</param>
        /// <returns></returns>
        private IValueSchema<object> GetSchemaForHint(SerializationTypeHint serializationHint)
        {
            Func<HibernatePropertySetCollection, IValueSchema<object>> f;
            if (_hintSchemaMap.TryGetValue(serializationHint, out f))
                return f.Invoke(this);
            throw new ArgumentException(string.Format("Invalid hint:{0}",serializationHint));
        }


        /// <summary>
        /// Deserializes the element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        private object DeserializeElement(HibernatePropertyElement element)
        {
            if (element == null)
                return null;
            Func<HibernatePropertySetCollection, HibernatePropertyElement, object> f;
            if (!_serializationMap.TryGetValue(element.SerializationHint, out f))
                throw new ArgumentException("Deserialization error.");
            return f.Invoke(this, element);
        }


        /// <summary>
        /// Gets the collection.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <returns></returns>
        private PropertyElementsCollection GetCollection(ISession session)
        {
            StorageCollectionDao dao=new StorageCollectionDao(session);
            return dao.GetCreateById(Id);
        }

        /// <summary>
        /// Runs the in session.
        /// </summary>
        /// <param name="block">The block.</param>
        /// <param name="exception">The exception.</param>
        private T RunInSession<T>(Func<ISession,T> block, Action<ISession,Exception> exception)
        {
            bool needClosing=false;
            bool trxCreated = false;
            ISession session;
            ITransaction transaction;
            try
            {
                session = _sessionFactory.GetCurrentSession();
                transaction = session.Transaction;
                if(transaction==null)
                {
                    transaction = session.BeginTransaction(IsolationLevel.Serializable);
                    trxCreated = true;
                }
            }
            catch(Exception)
            {
                needClosing = true;
                session = _sessionFactory.OpenSession();
                transaction = session.BeginTransaction(IsolationLevel.Serializable);
                trxCreated = true;
                CurrentSessionContext.Bind(session);
            }

            try
            {
                T value=block.Invoke(session);
                if(trxCreated)
                    transaction.Commit();
                return value;
            }
            catch(Exception ex)
            {
                try
                {
                    _logger.Error(ex);
                    exception.Invoke(session, ex);
                    throw;
                }
                finally
                {

                    if (trxCreated)
                        transaction.Rollback();
                }
            }
            finally
            {
                if (needClosing)
                {
                    CurrentSessionContext.Unbind(_sessionFactory);
                    session.Close();
                }
            }
            
        }
        #endregion

        #region Private Classes
        /// <summary>
        /// Private class of property sets
        /// </summary>
        private class PropertySchemaSet : PropertySchemaSetBase
        {
            private readonly HibernatePropertySetCollection _propertySet;
            /// <summary>
            /// Initializes a new instance of the <see cref="PropertySchemaSet"/> class.
            /// </summary>
            /// <param name="parent">The parent.</param>
            /// <param name="schemaFactory">The schema factory.</param>
            public PropertySchemaSet(
                HibernatePropertySetCollection parent,
                IPropertySchemaFactory schemaFactory)
                : base(schemaFactory)
            {
                _propertySet = parent;
            }


            #region Overrides of PropertySchemaSetBase

            /// <summary>
            /// Gets the schemas.
            /// </summary>
            public override IEnumerable<KeyValuePair<string, IValueSchema<object>>> Schemas
            {
                get
                {
                    return _propertySet.RunInSession(s =>
                           {
                                PropertyElementsCollection colection =_propertySet.GetCollection(s);
                                return colection.Elements.
                                    Where(e=>e.Value.Schema!=null && e.Value.Schema.SchemaBody!=null).
                                    Select(e =>
                                    new KeyValuePair<string, IValueSchema<object>>(e.Key,_propertySet.GetValueSchema(e.Value))).ToArray();
                           }, (s, e) => { });
                }
            }

            /// <summary>
            /// Removes the schema.
            /// </summary>
            /// <param name="name">The name.</param>
            /// <returns></returns>
            public override bool RemoveSchema(string name)
            {
                return _propertySet.RunInSession(s =>
                       {
                           StorageCollectionDao dao=new StorageCollectionDao(s);
                           PropertyElementsCollection collection = dao.GetById(_propertySet.Id);
                           if (collection == null)
                               return false;
                           HibernatePropertyElement element;
                           if(collection.Elements.TryGetValue(name,out element))
                           {
                               element.Schema.SchemaBody = null;
                               element.Schema.SchemaType = null;
                               dao.Store(collection);
                               return true;
                           }
                           return false;
                       }, (s, e) => { });
            }

            /// <summary>
            /// Removes all schemas.
            /// </summary>
            public override void RemoveAll()
            {
                _propertySet.RunInSession(s =>
                {
                    StorageCollectionDao dao = new StorageCollectionDao(s);
                    PropertyElementsCollection collection = dao.GetById(_propertySet.Id);
                    if (collection == null)
                        return false;
                    foreach (HibernatePropertyElement element in collection.Elements.Values)
                    {
                        element.Schema.SchemaBody = null;
                        element.Schema.SchemaType = null;
                    }
                    dao.Store(collection);
                    return true;
                }, (s, e) => { });
            }

            /// <summary>
            /// Create or save the schema
            /// </summary>
            /// <param name="name">The name.</param>
            /// <param name="wrapped">The wrapped.</param>
            protected override void OnSetSchema(string name, IValueSchema<object> wrapped)
            {
                _propertySet.RunInSession(s =>
                {
                    StorageCollectionDao dao = new StorageCollectionDao(s);
                    PropertyElementsCollection collection = dao.GetCreateById(_propertySet.Id);
                    HibernatePropertyElement element;
                    if(!collection.Elements.TryGetValue(name,out element))
                    {
                        element=new HibernatePropertyElement {Schema = new SchemaElement()};
                        collection.Elements[name] = element;
                    }
                    wrapped.Serializer.Serialize(wrapped.DefaultValue,element);
                    string body;
                    Type schemaType;
                    SchemaFactory.SerializeSchema(wrapped, out schemaType,out body);
                    element.Schema.SchemaBody = body;
                    element.Schema.SchemaType = schemaType.FullName;
                    dao.Store(collection);
                    return true;
                }, (s, e) => { });
            }

            /// <summary>
            /// Try to get the schema
            /// </summary>
            /// <param name="name">The name.</param>
            /// <param name="objSchema">The obj schema.</param>
            /// <returns></returns>
            protected override bool OnTryGetValue(string name, out IValueSchema<object> objSchema)
            {
                
                HibernatePropertyElement element;
                objSchema = _propertySet.RunInSession(s =>
                                              {
                                                  IValueSchema<object> schema = null;
                                                  StorageCollectionDao dao = new StorageCollectionDao(s);
                                                  PropertyElementsCollection collection  = dao.GetCreateById(_propertySet.Id);
                                                  if (collection.Elements.TryGetValue(name, out element) &&
                                                      element.Schema != null &&
                                                      !string.IsNullOrEmpty(element.Schema.SchemaBody))
                                                  {
                                                      schema =
                                                          SchemaFactory.DeserializeSchema(element.Schema.SchemaBody,
                                                                                          element.Schema.SchemaType);
                                                  }
                                                  return schema;
                                              }, (s, e) => {  });
                return objSchema != null;
            }

            #endregion

        }
        #endregion
    }
}
