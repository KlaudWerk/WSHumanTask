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
using System.Collections;
using System.Collections.Generic;

namespace Klaudwerk.PropertySet
{
    /// <summary>
    /// The base class for Value Set Collection
    /// </summary>
    public abstract class PropertySetCollectionBase:IPropertySetCollection
    {


        #region Implementation of IValueSetCollection

        /// <summary>
        /// Gets the Properties Schema Set.
        /// </summary>
        /// <value>The schemas.</value>
        public abstract IPropertySchemaSet Schemas { get; }

        #region Implementation of IValueSetCollection

        /// <summary>
        /// Adds the specified name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <param name="schema">The schema.</param>
        /// <returns></returns>
        public virtual T? Add<T>(string name, IValueSchema<T?> schema) where T : struct
        {
            if(IsReadOnly)
                throw new ArgumentException("The collection is read-only.");
            if(ContainsKey(name))
                throw new ArgumentException(string.Format("The property name {0} already exist.",name));
            T? retValue;
            if(schema.HasDefault)
            {
                retValue = schema.DefaultValue;
            }
            else
            {
                if(!schema.AllowNull)
                    throw new ArgumentException(string.Format("Null value is not allowed for {0}",name));
                retValue = default(T?);
            }
            AddWithSchema(name,retValue,schema.Wrap());
            return retValue;
        }

        /// <summary>
        /// Adds the specified name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <param name="schema">The schema.</param>
        /// <returns></returns>
        public virtual T? Add<T>(string name, T? value, IValueSchema<T?> schema) where T : struct
        {
            if (IsReadOnly)
                throw new ArgumentException("The collection is read-only.");

            if (ContainsKey(name))
                throw new ArgumentException(string.Format("The property name {0} already exist.", name));
            if(value==null && !schema.AllowNull)
                throw new ArgumentException(string.Format("Null value is not allowed for {0}", name));
            AddWithSchema(name,value,schema.Wrap());
            return value;
        }

        /// <summary>
        /// Adds the specified name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <param name="schema">The schema.</param>
        /// <returns></returns>
        public virtual T Add<T>(string name, IValueSchema<T> schema) where T : class
        {
            if (IsReadOnly)
                throw new ArgumentException("The collection is read-only.");

            if (ContainsKey(name))
                throw new ArgumentException(string.Format("The property name {0} already exist.", name));
            T value;
            if (schema.HasDefault)
            {
                value = schema.DefaultValue;
            }
            else
            {
                if (!schema.AllowNull)
                    throw new ArgumentException(string.Format("Null value is not allowed for {0}", name));
                value = null;
            }
            AddWithSchema(name, value, schema.Wrap());
            return value;
        }

        /// <summary>
        /// Adds the specified name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <param name="schema">The schema.</param>
        /// <returns></returns>
        public virtual T Add<T>(string name, T value, IValueSchema<T> schema) where T : class
        {
            if (IsReadOnly)
                throw new ArgumentException("The collection is read-only.");

            if (ContainsKey(name))
                throw new ArgumentException(string.Format("The property name {0} already exist.", name));
            if (ContainsKey(name))
                throw new ArgumentException(string.Format("The property name {0} already exist.", name));
            if (value == null && !schema.AllowNull)
                throw new ArgumentException(string.Format("Null value is not allowed for {0}", name));
            AddWithSchema(name, value, schema.Wrap());
            return value;
        }
        /// <summary>
        /// Adds the specified name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public virtual void Set<T>(string name, T value) where T : class
        {
            if (IsReadOnly)
                throw new ArgumentException("The collection is read-only.");
            if(!ContainsKey(name))
            {
                Add(name, value);
                return;
            }
            IValueSchema<T> schema;
            try
            {
                if (Schemas.TryGetSchema(name, out schema))
                    schema.Validate(value);
                 this[name]=value;
            }

            catch(InvalidCastException ex)
            {
                throw new PropertyValidationException(ex.Message,ex);
            }
        }

        /// <summary>
        /// Sets the specified name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public virtual void Set<T>(string name, T? value) where T : struct
        {
            if (IsReadOnly)
                throw new ArgumentException("The collection is read-only.");
            if (!ContainsKey(name))
            {
                Add(name, value);
                return;
            }
            IValueSchema<T?> schema;
            try
            {
                if (Schemas.TryGetSchema<T>(name, out schema))
                    schema.Validate(value);
                this[name]=value;
            }
            catch (InvalidCastException e)
            {
                throw new PropertyValidationException("type",e);
            }
        }

        /// <summary>
        /// Gets the value of the specified property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <returns>the value</returns>
        public virtual T Get<T>(string name) 
        {
            if(!ContainsKey(name))
                throw new ArgumentException("Property {0} does not exist.",name);
            IValueSchema<object> schema;
            object val = Schemas.TryGetSchema(name, out schema)?
                schema.Convert(this[name]): 
                this[name];
            return val==null? default(T) : (T) val;
        }

        /// <summary>
        /// Runs the validation on the collection
        /// </summary>
        public virtual void Validate()
        {
            List<string> errors = new List<string>();
            IEnumerator<KeyValuePair<string,object>> en=GetEnumerator();
            while(en.MoveNext())
            {
                IValueSchema<object> schema = Schemas.GetSchema(en.Current.Key);
                if (schema == null)
                    continue;
                try
                {
                    schema.Validate(en.Current.Value);
                }
                catch (PropertyValidationException ex)
                {
                    errors.Add(string.Format("{0} {1}", en.Current.Key, ex.Message));
                }
            }
            if(errors.Count>0)
                throw new PropertyValidationException(string.Format("Validation errors: {0}",string.Join("\n",errors)));
        }

        #endregion

        #endregion


        protected virtual void AddWithSchema(string name,object value,IValueSchema<object> schema)
        {
            Add(name,value);
            Schemas.SetSchema(name,schema);
        }

        protected virtual void AddValue(string name, object value)
        {
            Add(name, value);
        }

        #region Implementation of IEnumerable

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public abstract IEnumerator<KeyValuePair<string, object>> GetEnumerator();

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

        #region Implementation of ICollection<KeyValuePair<string,object>>

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
        public abstract void Add(KeyValuePair<string, object> item);

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only. </exception>
        public abstract void Clear();

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
        /// </summary>
        /// <returns>
        /// true if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false.
        /// </returns>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        public abstract bool Contains(KeyValuePair<string, object> item);

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param><param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param><exception cref="T:System.ArgumentNullException"><paramref name="array"/> is null.</exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than 0.</exception><exception cref="T:System.ArgumentException"><paramref name="array"/> is multidimensional.-or-The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.-or-Type <paramref name="T"/> cannot be cast automatically to the type of the destination <paramref name="array"/>.</exception>
        public abstract void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex);

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <returns>
        /// true if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
        public abstract bool Remove(KeyValuePair<string, object> item);

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <returns>
        /// The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        public abstract int Count { get; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only; otherwise, false.
        /// </returns>
        public abstract bool IsReadOnly { get; }

        #endregion

        #region Implementation of IDictionary<string,object>

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an element with the specified key.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an element with the key; otherwise, false.
        /// </returns>
        /// <param name="key">The key to locate in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.</param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception>
        public abstract bool ContainsKey(string key);

        /// <summary>
        /// Adds an element with the provided key and value to the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <param name="key">The object to use as the key of the element to add.</param><param name="value">The object to use as the value of the element to add.</param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception><exception cref="T:System.ArgumentException">An element with the same key already exists in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.</exception><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IDictionary`2"/> is read-only.</exception>
        public abstract void Add(string key, object value);

        /// <summary>
        /// Removes the element with the specified key from the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <returns>
        /// true if the element is successfully removed; otherwise, false.  This method also returns false if <paramref name="key"/> was not found in the original <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </returns>
        /// <param name="key">The key of the element to remove.</param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IDictionary`2"/> is read-only.</exception>
        public abstract bool Remove(string key);

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <returns>
        /// true if the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an element with the specified key; otherwise, false.
        /// </returns>
        /// <param name="key">The key whose value to get.</param><param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value"/> parameter. This parameter is passed uninitialized.</param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception>
        public abstract bool TryGetValue(string key, out object value);

        /// <summary>
        /// Gets or sets the element with the specified key.
        /// </summary>
        /// <returns>
        /// The element with the specified key.
        /// </returns>
        /// <param name="key">The key of the element to get or set.</param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception><exception cref="T:System.Collections.Generic.KeyNotFoundException">The property is retrieved and <paramref name="key"/> is not found.</exception><exception cref="T:System.NotSupportedException">The property is set and the <see cref="T:System.Collections.Generic.IDictionary`2"/> is read-only.</exception>
        public abstract object this[string key] { get; set; }

        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1"/> containing the keys of the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.Generic.ICollection`1"/> containing the keys of the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </returns>
        public abstract ICollection<string> Keys { get; }

        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1"/> containing the values in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.Generic.ICollection`1"/> containing the values in the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </returns>
        public abstract ICollection<object> Values { get; }

        #endregion
    }
}
