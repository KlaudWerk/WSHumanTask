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
using System.Data;
using NHibernate;

namespace Klaudwerk.PropertySet.Hibernate
{
    /// <summary>
    /// The Data Access Object for the Storage Collection
    /// </summary>
    internal class StorageCollectionDao
    {
        private readonly ISession _session;

        /// <summary>
        /// Initializes a new instance of the <see cref="StorageCollectionDao"/> class.
        /// </summary>
        /// <param name="session">The session.</param>
        public StorageCollectionDao(ISession session)
        {
            _session = session;
        }

        /// <summary>
        /// Gets the by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public PropertyElementsCollection GetById(Guid id)
        {
            return _session.Get<PropertyElementsCollection>(id);
        }
        /// <summary>
        /// Gets the collection by id. Create if doesn't exist
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public PropertyElementsCollection GetCreateById(Guid id)
        {
            PropertyElementsCollection collection = _session.Get<PropertyElementsCollection>(id);
            if(collection==null)
            {
                collection=new PropertyElementsCollection{Id=id};   
                Store(collection);
                collection = _session.Get<PropertyElementsCollection>(id);
            }
            return collection;
        }

        /// <summary>
        /// Stores the specified collection.
        /// </summary>
        /// <param name="collection">The collection.</param>
        public void Store(PropertyElementsCollection collection)
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