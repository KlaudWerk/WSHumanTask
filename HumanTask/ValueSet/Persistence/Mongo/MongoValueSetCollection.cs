using System;
using System.Collections.Generic;
using HumanTask.ValueSet.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace HumanTask.ValueSet.Persistence.Mongo
{
    /// <summary>
    /// Mongo persistent implementation of the ValueSet collection
    /// </summary>
    public class MongoValueSetCollection:ValueSetCollectionBase
    {
        private readonly MongoCollection<MongoPropertyElement> _mongoCollection;
        private readonly string _collectionName;
        private readonly JsonPropertySerializer _serializer;
        private readonly IPropertySchemaFactory _schemaFactory;

        /// <summary>
        /// Gets the name of the collection.
        /// </summary>
        /// <value>
        /// The name of the collection.
        /// </value>
        public string CollectionName
        {
            get { return _collectionName; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoValueSetCollection"/> class.
        /// </summary>
        /// <param name="mongoDatabase">The mongo database.</param>
        /// <param name="collectionId">The collection id.</param>
        /// <param name="schemaFactory">The schema factory.</param>
        public MongoValueSetCollection(MongoDatabase mongoDatabase, Guid collectionId,
            IPropertySchemaFactory schemaFactory):this(GetCreateCollection(mongoDatabase,collectionId),schemaFactory)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoValueSetCollection"/> class.
        /// </summary>
        /// <param name="mongoCollection">The mongo collection.</param>
        /// <param name="schemaFactory">The schema factory.</param>
        public MongoValueSetCollection(MongoCollection<MongoPropertyElement> mongoCollection,
            IPropertySchemaFactory schemaFactory)
        {
            _schemaFactory = schemaFactory;
            _mongoCollection = mongoCollection;
            _collectionName = _mongoCollection.Name;
            _serializer = new JsonPropertySerializer(_schemaFactory);
        }

        private static MongoCollection<MongoPropertyElement> GetCreateCollection(MongoDatabase mongoDatabase, Guid collectionId)
        {
            MongoCollection<MongoPropertyElement> mongoCollection;
            string collectionName = string.Format("vs-{0}", collectionId);
            if (mongoDatabase.CollectionExists(collectionName))
            {
                mongoCollection = mongoDatabase.GetCollection<MongoPropertyElement>(collectionName);
            }
            else
            {
                mongoCollection = mongoDatabase.GetCollection<MongoPropertyElement>(collectionName);
                mongoCollection.EnsureIndex(IndexKeys.Ascending("Name"), IndexOptions.SetUnique(true));
            }
            return mongoCollection;
        }
        /// <summary>
        /// Adds the with schema.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <param name="schema">The schema.</param>
        protected override void AddWithSchema(string name, object value, IValueSchema<object> schema)
        {
            MongoPropertyElement me = new MongoPropertyElement(_serializer.Serialize(name, value, schema));
            try
            {
                _mongoCollection.Insert(me, SafeMode.True);
            }
            catch(MongoSafeModeException ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }

        protected override IDictionary<string, object> Storage
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the Properties Schema Set.
        /// </summary>
        /// <value>The schemas.</value>
        public override IPropertySchemaSet Schemas
        {
            get { throw new NotImplementedException(); }
        }
   }
}
