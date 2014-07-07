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
using System.Linq;
using Klaudwerk.PropertySet.Serialization;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace Klaudwerk.PropertySet.Mongo
{
    /// <summary>
    /// Defines separate operations on value schemas
    /// </summary>
    public class MongoPropertySchemaSet : PropertySchemaSetBase
    {
        private readonly MongoCollection<BsonDocument> _collection;
        private readonly JsonPropertySerializer _serializer;
        /// <summary>
        /// Initializes a new instance of the <see cref="MongoPropertySchemaSet"/> class.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="schemaFactory">The schema factory.</param>
        public MongoPropertySchemaSet(
            MongoCollection<BsonDocument> collection,
            IPropertySchemaFactory schemaFactory)
            : base(schemaFactory)
        {
            _collection = collection;
            _serializer = new JsonPropertySerializer(schemaFactory);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoPropertySchemaSet"/> class.
        /// </summary>
        /// <param name="mongoDatabase">The mongo database.</param>
        /// <param name="collectionName">Name of the collection.</param>
        /// <param name="schemaFactory">The schema factory.</param>
        public MongoPropertySchemaSet(
            MongoDatabase mongoDatabase,
            string collectionName,
            IPropertySchemaFactory schemaFactory)
            : this(GetCreateCollection(mongoDatabase, collectionName), schemaFactory)
        {
        }

        private static MongoCollection<BsonDocument> GetCreateCollection(MongoDatabase mongoDatabase, string collectionName)
        {
            MongoCollection<BsonDocument> mongoCollection;
            if (mongoDatabase.CollectionExists(collectionName))
            {
                mongoCollection = mongoDatabase.GetCollection<BsonDocument>(collectionName);
            }
            else
            {
                mongoCollection = mongoDatabase.GetCollection<BsonDocument>(collectionName);
                mongoCollection.EnsureIndex(IndexKeys.Ascending("key"), IndexOptions.SetUnique(true));
            }
            return mongoCollection;
        }

        #region Overrides of PropertySchemaSetBase

        /// <summary>
        /// Gets the schemas.
        /// </summary>
        public override IEnumerable<KeyValuePair<string, IValueSchema<object>>> Schemas
        {
            get
            {
                return _collection.FindAll().Select(
                    e => new KeyValuePair<string, IValueSchema<object>>(e[MongoKeys.KeyField].AsString, InstantiatSchema(e)));
            }
        }

        /// <summary>
        /// Removes the schema.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public override bool RemoveSchema(string name)
        {
            QueryDocument query = new QueryDocument(MongoKeys.KeyField, name);
            SafeModeResult result = _collection.Remove(query, RemoveFlags.Single, SafeMode.True);
            return result.DocumentsAffected > 0;
        }

        /// <summary>
        /// Removes all schemas.
        /// </summary>
        public override void RemoveAll()
        {
            _collection.RemoveAll(SafeMode.True);
        }

        /// <summary>
        /// Create or save the schema
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="wrapped">The wrapped.</param>
        protected override void OnSetSchema(string name, IValueSchema<object> wrapped)
        {
            QueryDocument query = new QueryDocument(MongoKeys.KeyField, name);
            UpdateBuilder update = GetUpdates(wrapped);
            _collection.Update(query, update, UpdateFlags.Upsert, SafeMode.True);
        }

        /// <summary>
        /// Gets the updates.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        protected UpdateBuilder GetUpdates(IValueSchema<object> value)
        {
            Type schemaType;
            string schemaValue;
            _serializer.SerializeSchema(value, out schemaType, out schemaValue);
            UpdateBuilder builder = Update.Set(MongoKeys.SchemaType, schemaType.FullName).
                Set(MongoKeys.SchemaBody, schemaValue);
            return builder;
        }


        /// <summary>
        /// Try to get the schema
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="objSchema">The obj schema.</param>
        /// <returns></returns>
        protected override bool OnTryGetValue(string name, out IValueSchema<object> objSchema)
        {
            QueryDocument query = new QueryDocument(MongoKeys.KeyField, name);
            MongoCursor<BsonDocument> element = _collection.Find(query);
            if (element.Count() == 0)
            {
                objSchema = null;
                return false;
            }
            return (objSchema = InstantiatSchema(element.FirstOrDefault())) != null;
        }
        #endregion

        private  IValueSchema<object> InstantiatSchema(BsonDocument document)
        {
            SchemaElement se=new SchemaElement();
            se.FillSchemaromBsonDocument(document);
            return (se.SchemaBody == null && se.SchemaType == null) ?
                null : _serializer.DeserializeSchema(se);
        }
    }
}