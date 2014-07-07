using System;
using System.Collections.Generic;
using System.Data;
using NHibernate;

namespace Klaudwerk.PropertySet.Hibernate
{
    /// <summary>
    /// Property Collection
    /// </summary>
    public class PropertyCollection
    {
        /// <summary>
        /// Gets or sets the unique collection id.
        /// </summary>
        /// <value>The id.</value>
        public virtual Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>The version.</value>
        public virtual int Version { get; set; }

        /// <summary>
        /// Gets or sets the elements.
        /// </summary>
        /// <value>The elements.</value>
        public virtual IDictionary<string, HibernatePropertyElement> Elements { get; set; }

    }

    /// <summary>
    /// The Data Access Object for the Storage Collection
    /// </summary>
    public class StorageCollectionDao
    {
        private readonly ISession _session;

        public StorageCollectionDao(ISession session)
        {
            _session = session;
        }

        public PropertyCollection GetById(Guid id)
        {
            return (PropertyCollection)_session.Load(typeof (PropertyCollection), id);
        }

        /// <summary>
        /// Stores the specified collection.
        /// </summary>
        /// <param name="collection">The collection.</param>
        public void Store(PropertyCollection collection)
        {
            bool requireTransactionFinalization = false;
            ITransaction transaction;
            if (_session.Transaction == null)
            {
                transaction = _session.BeginTransaction(IsolationLevel.Serializable);
                requireTransactionFinalization = true;
            }
            else
            {
                transaction = _session.Transaction;
            }
            try
            {
               _session.SaveOrUpdate(collection); 
                if(requireTransactionFinalization)
                    transaction.Commit();
            }
            catch
            {
                if(requireTransactionFinalization)
                    transaction.Rollback();
                throw;
            }
        }
    }
}
