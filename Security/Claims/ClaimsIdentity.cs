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
using System.IdentityModel.Tokens;
using System.Linq;
using KlaudWerk.Common;
using Microsoft.IdentityModel.Claims;

namespace KlaudWerk.Security.Claims
{
    /// <summary>
    /// Implementation of a Claims Identity
    /// </summary>
    public class ClaimsIdentity:IClaimsIdentity, IEquatable<ClaimsIdentity>
    {
        private readonly ClaimCollection _claims;
        private readonly IdentityId _account;

        /// <summary>
        /// Gets the mapped id.
        /// </summary>
        /// <value>The mapped id.</value>
        public IdentityId MappedId
        {
            get { return _account; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClaimsIdentity"/> class.
        /// </summary>
        /// <param name="account">The account.</param>
        public ClaimsIdentity(IdentityId account)
        {
            account.NotNull();
            _account = account;
            _claims=new ClaimCollection(this)
                        {
                            new Claim(ClaimType.MappedId.ToString(),account.ToString()),
                        };
            Actor = this;
        }
        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="identity">identity to copy</param>
        private ClaimsIdentity(ClaimsIdentity identity):this(identity._account)
        {
            identity.NotNull();
            _claims=new ClaimCollection(this);
            _claims.AddRange(identity._claims.AsEnumerable());
        }
        /// <summary>
        /// Clone the Claims Identity
        /// </summary>
        /// <returns></returns>
        public IClaimsIdentity Copy()
        {
            return new ClaimsIdentity(this);
        }
        /// <summary>
        /// Collection of claims associated
        /// with the identity
        /// </summary>
        public ClaimCollection Claims
        {
            get { return _claims; }
        }
        /// <summary>
        /// Actor 
        /// </summary>
        public IClaimsIdentity Actor
        {
            get; set;
        }
        /// <summary>
        /// Identity label
        /// </summary>
        public string Label
        {
            get; set;
        }
        /// <summary>
        /// Name claim type is fixed for this implementation
        /// </summary>
        public string NameClaimType
        {
            get { return ClaimType.AccountName.ToString(); }
            set {}
            
        }
        /// <summary>
        /// Role claim type is fixed for this implementation
        /// </summary>
        public string RoleClaimType
        {
            get { return ClaimType.Role.ToString(); }
            set {}
        }
        /// <summary>
        /// Preserved Bootstrap token 
        /// </summary>
        public SecurityToken BootstrapToken
        {
            get; set;
        }
        /// <summary>
        /// Gets the name of the current user.
        /// </summary>
        /// <returns>
        /// The name of the user on whose behalf the code is running.
        /// </returns>
        public string Name
        {
            get { return this.GetValuesOfClaim(ClaimType.AccountName.ToString()).FirstOrDefault(); }
        }

        /// <summary>
        /// Gets the type of authentication used.
        /// </summary>
        /// <returns>
        /// The type of authentication used to identify the user.
        /// </returns>
        public string AuthenticationType
        {
            get { return this.GetValuesOfClaim(ClaimType.Authentication.ToString()).FirstOrDefault(); }
        }

        /// <summary>
        /// Gets a value that indicates whether the user has been authenticated.
        /// </summary>
        /// <returns>
        /// true if the user was authenticated; otherwise, false.
        /// </returns>
        public bool IsAuthenticated { get; set; }

        #region IEquatable
        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.
        ///                 </param>
        public bool Equals(ClaimsIdentity other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._account, _account);
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
            if (obj.GetType() != typeof (ClaimsIdentity)) return false;
            return Equals((ClaimsIdentity) obj);
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
            return (_account.GetHashCode());
        }

        public static bool operator ==(ClaimsIdentity left, ClaimsIdentity right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ClaimsIdentity left, ClaimsIdentity right)
        {
            return !Equals(left, right);
        }
        #endregion
    }
}
