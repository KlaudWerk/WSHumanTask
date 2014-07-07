using System;
using System.Collections.Generic;
using Klaudwerk.PropertySet.Serialization;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace Klaudwerk.PropertySet.Mongo
{
    public class MongoPropertySchema : PropertySchemaSetBase
    {
        private readonly MongoCollection<BsonDocument> _collection;
        private readonly IDictionary<string, IValueSchema<object>> _store;
        private readonly JsonPropertySerializer _serializer;
        /// <summary>
        /// Initializes a new instance of the <see cref="MongoPropertySchema"/> class.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="schemaFactory">The schema factory.</param>
        public MongoPropertySchema(
            MongoCollection<BsonDocument> collection,
            IPropertySchemaFactory schemaFactory)
            : base(schemaFactory)
        {
            _collection = collection;
            _serializer = new JsonPropertySerializer(schemaFactory);
            _store = new MongoSchemaStorageDictionary(_collection,_serializer);
        }

        /// <summary>
        /// Gets the schema store.
        /// </summary>
        /// <value>The schema store.</value>
        protected override IDictionary<string, IValueSchema<object>> SchemaStore
        {
            get { return _store; }
        }


        private class MongoSchemaStorageDictionary:MongoCollectionDictionary<string,IValueSchema<object>>
        {
            private readonly JsonPropertySerializer _serializer;
            public MongoSchemaStorageDictionary(MongoCollection<BsonDocument> collection,JsonPropertySerializer serializer) : base(collection)
            {
                _serializer = serializer;
            }

            protected override BsonDocument ConvertToCollectionElementValue(string key, IValueSchema<object> value)
            {
                Type schemaType;
                string schemaValue;
                _serializer.SerializeSchema(value,out schemaType,out schemaValue);
                BsonDocument document=new BsonDocument
                                          {
                                              new BsonElement("key", key),
                                              new BsonElement("sc", schemaValue),
                                              new BsonElement("st", schemaType.FullName)
                                          };
                return document;
            }

            protected override BsonValue ToBsonValue(string key)
            {
                return new BsonString(key);
            }

            /// <summary>
            /// Gets the name of the key field.
            /// </summary>
            /// <returns></returns>
            protected override string GetKeyField()
            {
                return "key";
            }

            /// <summary>
            /// Gets the key field value.
            /// </summary>
            /// <param name="value">The value.</param>
            /// <returns></returns>
            protected override string GetKeyFieldValue(BsonDocument value)
            {
                return value["key"].AsString;
            }
            /// <summary>
            /// Gets the updates.
            /// </summary>
            /// <param name="key">The key.</param>
            /// <param name="value">The value.</param>
            /// <returns></returns>
            protected override UpdateBuilder GetUpdates(string key, IValueSchema<object> value)
            {
                Type schemaType;
                string schemaValue;
                _serializer.SerializeSchema(value, out schemaType, out schemaValue);
                return Update.Set("st", schemaType.FullName).Set("sc", schemaValue);
            }

            protected override IValueSchema<object> ConvertValue(BsonDocument mongoElement)
            {
                PropertyElement pe = new PropertyElement
                                         {
                                             SchemaBody = mongoElement["sc"].AsString,
                                             SchemaType = mongoElement["st"].AsString
                                         };
                return _serializer.DeserializeSchema(pe);
            }
        }
    }
}