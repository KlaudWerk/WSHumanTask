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
using System.Data;
using NHibernate;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;

namespace HumanTask.Hibernate.Helpers
{
    /// <summary>
    /// A User Type for type-safe guid-based IDs
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class GuidIdType<T>:IUserType
    {
        protected abstract Guid? GetValue(object val);
        protected abstract T InstValue(object o);
        /// <summary>
        /// Compare two instances of the class mapped by this type for persistent "equality"
        ///             ie. equality of persistent state
        /// </summary>
        /// <param name="x"/><param name="y"/>
        /// <returns/>
        public bool Equals(object x, object y)
        {
            return x==null?y==null:x.Equals(y);
        }

        /// <summary>
        /// Get a hashcode for the instance, consistent with persistence "equality"
        /// </summary>
        public int GetHashCode(object x)
        {
            return x == null ? 0 : x.GetHashCode();
        }

        /// <summary>
        /// Retrieve an instance of the mapped class from a JDBC resultset.
        ///             Implementors should handle possibility of null values.
        /// </summary>
        /// <param name="rs">a IDataReader</param><param name="names">column names</param><param name="owner">the containing entity</param>
        /// <returns/>
        /// <exception cref="T:NHibernate.HibernateException">HibernateException</exception>
        public object NullSafeGet(IDataReader rs, string[] names, object owner)
        {
            return InstValue(NHibernateUtil.Guid.NullSafeGet(rs,names));

        }
        /// <summary>
        /// Write an instance of the mapped class to a prepared statement.
        ///             Implementors should handle possibility of null values.
        ///             A multi-column type should be written to parameters starting from index.
        /// </summary>
        /// <param name="cmd">a IDbCommand</param><param name="value">the object to write</param><param name="index">command parameter index</param><exception cref="T:NHibernate.HibernateException">HibernateException</exception>
        public void NullSafeSet(IDbCommand cmd, object value, int index)
        {
            NHibernateUtil.Guid.NullSafeSet(cmd, value == null ? null :(object) GetValue(value),index);
        }

        /// <summary>
        /// Return a deep copy of the persistent state, stopping at entities and at collections.
        /// </summary>
        /// <param name="value">generally a collection element or entity field</param>
        /// <returns>
        /// a copy
        /// </returns>
        public virtual object DeepCopy(object value)
        {
            return InstValue(GetValue(value));
        }


        /// <summary>
        /// During merge, replace the existing (<paramref name="target"/>) value in the entity
        ///             we are merging to with a new (<paramref name="original"/>) value from the detached
        ///             entity we are merging. For immutable objects, or null values, it is safe to simply
        ///             return the first parameter. For mutable objects, it is safe to return a copy of the
        ///             first parameter. For objects with component values, it might make sense to
        ///             recursively replace component values.
        /// </summary>
        /// <param name="original">the value from the detached entity being merged</param><param name="target">the value in the managed entity</param><param name="owner">the managed entity</param>
        /// <returns>
        /// the value to be merged
        /// </returns>
        public virtual object Replace(object original, object target, object owner)
        {
            return original;
        }

        /// <summary>
        /// Reconstruct an object from the cacheable representation. At the very least this
        ///             method should perform a deep copy if the type is mutable. (optional operation)
        /// </summary>
        /// <param name="cached">the object to be cached</param><param name="owner">the owner of the cached object</param>
        /// <returns>
        /// a reconstructed object from the cachable representation
        /// </returns>
        public virtual object Assemble(object cached, object owner)
        {
            return cached;
        }

        /// <summary>
        /// Transform the object into its cacheable representation. At the very least this
        ///             method should perform a deep copy if the type is mutable. That may not be enough
        ///             for some implementations, however; for example, associations must be cached as
        ///             identifier values. (optional operation)
        /// </summary>
        /// <param name="value">the object to be cached</param>
        /// <returns>
        /// a cacheable representation of the object
        /// </returns>
        public virtual object Disassemble(object value)
        {
            return value;
        }

        private static readonly SqlType[] _types=new []{ new SqlType(DbType.Guid) };
        /// <summary>
        /// The SQL types for the columns mapped by this type. 
        /// </summary>
        public SqlType[] SqlTypes
        {
            get { return _types; }
        }

        /// <summary>
        /// The type returned by <c>NullSafeGet()</c>
        /// </summary>
        public Type ReturnedType
        {
            get { return typeof(T); }
        }

        /// <summary>
        /// Are objects of this type mutable?
        /// </summary>
        public bool IsMutable
        {
            get { return false; }
        }
    }
}