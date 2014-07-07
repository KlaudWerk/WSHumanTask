using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using HumanTask.Mongo.Exceptions;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace HumanTask.Mongo
{
    /// <summary>
    /// Data Access object for Mongo database
    /// </summary>
    internal class MongoTaskDao
    {
        private MongoDatabase _database;
        private MongoCollection<MongoTaskCollectionElement> _collection;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoTaskDao"/> class.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <param name="collectionName">Name of the collection.</param>
        public MongoTaskDao(MongoDatabase database,
            string collectionName)
        {
            _database = database;
            _collection = GetCreateCollection(database, collectionName);
        }

        /// <summary>
        /// Creates the task.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="status">The status.</param>
        /// <param name="name">The name.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="priority">The priority.</param>
        /// <param name="isSkippable">if set to <c>true</c> [is skippable].</param>
        /// <param name="created">The created.</param>
        /// <param name="initiator">The initiator.</param>
        public void CreateTask(
            Guid id,
            TaskStatus status, 
            string name, 
            string subject, 
            Priority priority,
            bool isSkippable, 
            DateTime created, 
            IIdentity initiator)
        {
            MongoTaskCollectionElement element=new MongoTaskCollectionElement
                                                   {
                                                       Id = id.ToString(),
                                                       Created = created,
                                                       Name = name,
                                                       Subject = subject,
                                                       Priority =(int) priority,
                                                       Status = (int)status,
                                                       Skippable = isSkippable,
                                                       //InitiatorId = initiator.GetMappedId()
                                                   };
            try
            {
                _collection.Insert(element, SafeMode.True);
            }
            catch (MongoSafeModeException ex)
            {
                if (ex.CommandResult.Response.ElementCount != 0)
                {
                    BsonValue val =
                        ex.CommandResult.Response.Elements.Where(e => e.Name.Equals("code")).Select(e => e.Value).
                            FirstOrDefault();
                    if (val.AsInt32 == 11000)
                    {
                        throw new DuplicateKeyException();
                    }
                }
                else
                    throw;
            }
        }

        /// <summary>
        /// Try to fidn a task the by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public bool TryFidnById(Guid id, out MongoTaskCollectionElement element)
        {
            element = _collection.FindOneById(id.ToString());
            return null!=element;
        }

        /// <summary>
        /// Retrieve existing or create new collection
        /// </summary>
        /// <param name="mongoDatabase">The mongo database.</param>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns></returns>
        private static MongoCollection<MongoTaskCollectionElement> GetCreateCollection(MongoDatabase mongoDatabase, string collectionName)
        {
            MongoCollection<MongoTaskCollectionElement> mongoCollection;
            if (mongoDatabase.CollectionExists(collectionName))
            {
                mongoCollection = mongoDatabase.GetCollection<MongoTaskCollectionElement>(collectionName);
            }
            else
            {
                mongoCollection = mongoDatabase.GetCollection<MongoTaskCollectionElement>(collectionName);
                mongoCollection.EnsureIndex(IndexKeys.Ascending("Id"), IndexOptions.SetUnique(true));
            }
            return mongoCollection;
        }
    }
}
