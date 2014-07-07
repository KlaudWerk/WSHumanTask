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
using System.Runtime.Serialization;

namespace KlaudWerk.Security
{
    /// <summary>
    /// Type-safe immutable Account Id 
    /// </summary>
    [Serializable]
    [DataContract]
    public class AccountId : IEquatable<AccountId>
    {
        [DataMember]
        private readonly string _authSystem;
        [DataMember]
        private readonly string _nativeId;
        [DataMember]
        private readonly IdentityId _mappedId;

        /// <summary>
        /// Gets the mapped id.
        /// </summary>
        /// <value>The mapped id.</value>
        public IdentityId MappedId
        {
            get { return _mappedId; }
        }

        /// <summary>
        /// Gets the authentication system.
        /// </summary>
        /// <value>The auth system.</value>
        public string AuthSystem
        {
            get { return _authSystem; }
        }

        /// <summary>
        /// Gets the Account's Native Id
        /// </summary>
        /// <value>The auth system id.</value>
        public string NativeId
        {
            get { return _nativeId; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountId"/> class.
        /// </summary>
        /// <param name="authSystem">The auth system.</param>
        /// <param name="nativeId">The auth system id.</param>
        /// <param name="mappedId">The mapped id.</param>
        public AccountId(string authSystem, string nativeId, IdentityId mappedId)
        {
            _authSystem = authSystem;
            _nativeId = nativeId;
            _mappedId = mappedId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountId"/> class.
        /// </summary>
        /// <param name="authSystem">The auth system.</param>
        /// <param name="nativeId">The native id.</param>
        public AccountId(string authSystem, string nativeId):this(authSystem,nativeId,null)
        {
        }

        #region IEquatable
        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.
        ///                 </param>
        public bool Equals(AccountId other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._authSystem, _authSystem) && Equals(other._nativeId, _nativeId) && other._mappedId.Equals(_mappedId);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>. 
        ///                 </param><exception cref="T:System.NullReferenceException">The <paramref name="obj"/> parameter is null.
        ///                 </exception><filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (AccountId)) return false;
            return Equals((AccountId) obj);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            unchecked
            {
                int result = (_authSystem != null ? _authSystem.GetHashCode() : 0);
                result = (result*397) ^ (_nativeId != null ? _nativeId.GetHashCode() : 0);
                result = (result*397) ^ (_mappedId!=null ? _mappedId.GetHashCode() : 0);
                return result;
            }
        }

        public static bool operator ==(AccountId left, AccountId right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(AccountId left, AccountId right)
        {
            return !Equals(left, right);
        }
        #endregion
    }
}