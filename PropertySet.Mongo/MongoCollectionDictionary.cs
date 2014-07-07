﻿/**
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace Klaudwerk.PropertySet.Mongo
{
    /// <summary>
    /// Access a Mongo Collection as a Dictionary
    /// 
    /// </summary>
    public abstract class MongoCollectionDictionary<TKey,TVal>:IDictionary<TKey,TVal> where TVal:class
    {
        private readonly MongoCollection<BsonDocument> _collection;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoCollectionDictionary&lt;TE, T&gt;"/> class.
        /// </summary>
        /// <param name="collection">The collection.</param>
        protected MongoCollectionDictionary(MongoCollection<BsonDocument> collection)
        {
            _collection = collection;
        }

        #region Implementation of IEnumerable

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<KeyValuePair<TKey, TVal>> GetEnumerator()
        {
            return _collection.FindAll().Select(e => new KeyValuePair<TKey, TVal>(GetKeyFieldValue(e),ConvertValue(e))).GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Implementation of ICollection<KeyValuePair<string,T>>

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        ///                 </param><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        ///                 </exception>
        public void Add(KeyValuePair<TKey, TVal> item)
        {
            Add(item.Key,item.Value);
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only. 
        ///                 </exception>
        public void Clear()
        {
            _collection.RemoveAll(SafeMode.True);
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
        /// </summary>
        /// <returns>
        /// true if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false.
        /// </returns>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        ///                 </param>
        public bool Contains(KeyValuePair<TKey, TVal> item)
        {
            return ContainsKey(item.Key);
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.
        ///                 </param><param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.
        ///                 </param><exception cref="T:System.ArgumentNullException"><paramref name="array"/> is null.
        ///                 </exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than 0.
        ///                 </exception><exception cref="T:System.ArgumentException"><paramref name="array"/> is multidimensional.
        ///                     -or-
        ///                 <paramref name="arrayIndex"/> is equal to or greater than the length of <paramref name="array"/>.
        ///                     -or-
        ///                     The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.
        ///                     -or-
        ///                     Type <paramref name="T"/> cannot be cast automatically to the type of the destination <paramref name="array"/>.
        ///                 </exception>
        public void CopyTo(KeyValuePair<TKey, TVal>[] array, int arrayIndex)
        {
            MongoCursor<BsonDocument> cursor = _collection.FindAll();
            IEnumerable<KeyValuePair<TKey,TVal>> en= cursor.Select(e => new KeyValuePair<TKey, TVal>(GetKeyFieldValue(e), ConvertValue(e)));
            foreach (KeyValuePair<TKey, TVal> pair in en)
            {
                array[arrayIndex++] = pair;
            }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <returns>
        /// true if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        ///                 </param><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        ///                 </exception>
        public bool Remove(KeyValuePair<TKey, TVal> item)
        {
            QueryDocument query = new QueryDocument(GetKeyField(),ToBsonValue(item.Key));
            SafeModeResult result = _collection.Remove(query, RemoveFlags.Single, SafeMode.True);
            return result.Ok;
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <returns>
        /// The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        public int Count
        {
            get { return (int)_collection.Count(); }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only; otherwise, false.
        /// </returns>
        public bool IsReadOnly
        {
            get { return false; }
        }

        #endregion

        #region Implementation of IDictionary<string,T>

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an element with the specified key.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an element with the key; otherwise, false.
        /// </returns>
        /// <param name="key">The key to locate in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        ///                 </param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.
        ///                 </exception>
        public bool ContainsKey(TKey key)
        {
            QueryDocument query = new QueryDocument(GetKeyField(),ToBsonValue(key));
            return _collection.Find(query).Count() != 0;
        }
        /// <summary>
        /// Adds an element with the provided key and value to the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <param name="key">The object to use as the key of the element to add.
        ///                 </param><param name="value">The object to use as the value of the element to add.
        ///                 </param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.
        ///                 </exception><exception cref="T:System.ArgumentException">An element with the same key already exists in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        ///                 </exception><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IDictionary`2"/> is read-only.
        ///                 </exception>
        public void Add(TKey key, TVal value)
        {
            try
            {
                _collection.Insert(ConvertToCollectionElementValue(key, value), SafeMode.True);
            }
            catch(MongoSafeModeException ex)
            {
                if(ex.CommandResult.Response.ElementCount!=0)
                {
                    BsonValue val = ex.CommandResult.Response.Elements.Where(e => e.Name.Equals("code")).Select(e => e.Value).FirstOrDefault();
                    if(val.AsInt32==11000)
                    {
                        throw new ArgumentException(string.Format("Key {0} already exists in the dictionary.",key));
                    }
                }
                throw new NotSupportedException(ex.Message,ex);
            }
        }

        protected abstract BsonDocument ConvertToCollectionElementValue(TKey key, TVal value);

        /// <summary>
        /// Removes the element with the specified key from the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <returns>
        /// true if the element is successfully removed; otherwise, false.  This method also returns false if <paramref name="key"/> was not found in the original <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </returns>
        /// <param name="key">The key of the element to remove.
        ///                 </param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.
        ///                 </exception><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IDictionary`2"/> is read-only.
        ///                 </exception>
        public bool Remove(TKey key)
        {
            QueryDocument query = new QueryDocument(GetKeyField(),ToBsonValue(key));
            SafeModeResult result = _collection.Remove(query, RemoveFlags.Single, SafeMode.True);
            return result.DocumentsAffected>0;
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <returns>
        /// true if the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an element with the specified key; otherwise, false.
        /// </returns>
        /// <param name="key">The key whose value to get.
        ///                 </param><param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value"/> parameter. This parameter is passed uninitialized.
        ///                 </param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.
        ///                 </exception>
        public bool TryGetValue(TKey key, out TVal value)
        {
            QueryDocument query = new QueryDocument(GetKeyField(), ToBsonValue(key));
            MongoCursor<BsonDocument> element = _collection.Find(query);
            if (element.Count() == 0)
            {
                value = default(TVal);
                return false;

            }
            return TryConvertValue(element.FirstOrDefault(), out value);

        }

        /// <summary>
        /// Gets or sets the element with the specified key.
        /// </summary>
        /// <returns>
        /// The element with the specified key.
        /// </returns>
        /// <param name="key">The key of the element to get or set.
        ///                 </param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.
        ///                 </exception><exception cref="T:System.Collections.Generic.KeyNotFoundException">The property is retrieved and <paramref name="key"/> is not found.
        ///                 </exception><exception cref="T:System.NotSupportedException">The property is set and the <see cref="T:System.Collections.Generic.IDictionary`2"/> is read-only.
        ///                 </exception>
        public TVal this[TKey key]
        {
            get
            {
                QueryDocument query = new QueryDocument(GetKeyField(), ToBsonValue(key));
                return ConvertValue(_collection.FindOne(query));
            }
            set
            {
                QueryDocument query = new QueryDocument(GetKeyField(), ToBsonValue(key));
                UpdateBuilder update = GetUpdates(key, value);
                _collection.Update(query, update, UpdateFlags.Upsert, SafeMode.True);
            }
        }
        protected abstract BsonValue ToBsonValue(TKey key);

        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1"/> containing the keys of the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.Generic.ICollection`1"/> containing the keys of the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </returns>
        public ICollection<TKey> Keys
        {
            get
            {
                MongoCursor<BsonDocument> cursor = _collection.FindAll();
                return new ReadOnlyCollection<TKey>(cursor.Select( GetKeyFieldValue));
            }
        }

        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1"/> containing the values in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.Generic.ICollection`1"/> containing the values in the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </returns>
        public ICollection<TVal> Values
        {
            get
            {
                MongoCursor<BsonDocument> cursor = _collection.FindAll();
                return new ReadOnlyCollection<TVal>(cursor.Select(ConvertValue));
            }
        }

        #endregion

        /// <summary>
        /// Gets the name of the key field.
        /// </summary>
        /// <returns></returns>
        protected abstract string GetKeyField();
        /// <summary>
        /// Gets the key field value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        protected abstract TKey GetKeyFieldValue(BsonDocument value);
        /// <summary>
        /// Gets the updates.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        protected abstract UpdateBuilder GetUpdates(TKey key,TVal value);
        protected abstract TVal ConvertValue(BsonDocument mongoElement);
        protected abstract bool TryConvertValue(BsonDocument mongoElement,out TVal value);
        /// <summary>
        /// Implementation of read-only collection that reads information from the cursor
        /// </summary>
        private class ReadOnlyCollection<TC>:ICollection<TC>
        {
            private readonly IEnumerable<TC> _collection;
            public ReadOnlyCollection(IEnumerable<TC> collection)
            {
                _collection = collection;
            }

            #region Implementation of IEnumerable

            /// <summary>
            /// Returns an enumerator that iterates through the collection.
            /// </summary>
            /// <returns>
            /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
            /// </returns>
            /// <filterpriority>1</filterpriority>
            public IEnumerator<TC> GetEnumerator()
            {
                return _collection.GetEnumerator();
            }

            /// <summary>
            /// Returns an enumerator that iterates through a collection.
            /// </summary>
            /// <returns>
            /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
            /// </returns>
            /// <filterpriority>2</filterpriority>
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #endregion

            #region Implementation of ICollection<T>

            /// <summary>
            /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
            /// </summary>
            /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
            ///                 </param><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
            ///                 </exception>
            public void Add(TC item)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
            /// </summary>
            /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only. 
            ///                 </exception>
            public void Clear()
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
            /// </summary>
            /// <returns>
            /// true if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false.
            /// </returns>
            /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
            ///                 </param>
            public bool Contains(TC item)
            {
                return _collection.Contains(item);
            }

            /// <summary>
            /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
            /// </summary>
            /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.
            ///                 </param><param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.
            ///                 </param><exception cref="T:System.ArgumentNullException"><paramref name="array"/> is null.
            ///                 </exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than 0.
            ///                 </exception><exception cref="T:System.ArgumentException"><paramref name="array"/> is multidimensional.
            ///                     -or-
            ///                 <paramref name="arrayIndex"/> is equal to or greater than the length of <paramref name="array"/>.
            ///                     -or-
            ///                     The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.
            ///                     -or-
            ///                     Type <paramref name="T"/> cannot be cast automatically to the type of the destination <paramref name="array"/>.
            ///                 </exception>
            public void CopyTo(TC[] array, int arrayIndex)
            {
                foreach (TC val in _collection)
                {
                    array[arrayIndex++] = val;
                }
            }

            /// <summary>
            /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
            /// </summary>
            /// <returns>
            /// true if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
            /// </returns>
            /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
            ///                 </param><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
            ///                 </exception>
            public bool Remove(TC item)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
            /// </summary>
            /// <returns>
            /// The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
            /// </returns>
            public int Count
            {
                get { return _collection.Count(); }
            }

            /// <summary>
            /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
            /// </summary>
            /// <returns>
            /// true if the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only; otherwise, false.
            /// </returns>
            public bool IsReadOnly
            {
                get { return true; }
            }

            #endregion
        }
    }
}