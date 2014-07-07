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
using System.Linq;
using System.Security.Principal;
using Microsoft.IdentityModel.Claims;

namespace KlaudWerk.Security.Claims
{
    /// <summary>
    /// Simple Claims Principal implementation
    /// </summary>
    public class ClaimsPrincipal:IClaimsPrincipal, IEquatable<ClaimsPrincipal>
    {
        private readonly ClaimsIdentityCollection _claimsIdentities = new ClaimsIdentityCollection();
        private readonly IClaimsIdentity _identity;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClaimsPrincipal"/> class.
        /// </summary>
        /// <param name="identity">The identity.</param>
        public ClaimsPrincipal(IClaimsIdentity identity)
        {
            _identity = identity;
            _claimsIdentities.Add(_identity);
        }
        /// <summary>
        /// Determines whether the current principal belongs to the specified role.
        /// </summary>
        /// <returns>
        /// true if the current principal is a member of the specified role; otherwise, false.
        /// </returns>
        /// <param name="role">The name of the role for which to check membership. </param>
        public bool IsInRole(string role)
        {
            var c = from claim in _identity.Claims
                    where claim.ClaimType == ClaimType.Role.ToString() &&
                          claim.Value == role
                    select 1;
            return c.Count() != 0;
        }

        /// <summary>
        /// Determines whether the principal is a member of the specified group.
        /// </summary>
        /// <param name="groupName">The group.</param>
        /// <returns>
        /// 	<c>true</c> if [is member of] [the specified group]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsMemberOf(string groupName)
        {
            var c = from claim in _identity.Claims
                    where claim.ClaimType == ClaimType.Group.ToString() &&
                          claim.Value == groupName
                    select 1;
            return c.Count() != 0;
            
        }
        /// <summary>
        /// Gets the identity of the current principal.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Security.Principal.IIdentity"/> object associated with the current principal.
        /// </returns>
        public IIdentity Identity
        {
            get { return _identity; }
        }

        /// <summary>
        /// Copies this instance.
        /// </summary>
        /// <returns></returns>
        public IClaimsPrincipal Copy()
        {
            return new ClaimsPrincipal(_identity);
        }

        /// <summary>
        /// Gets the identities.
        /// </summary>
        /// <value>The identities.</value>
        public ClaimsIdentityCollection Identities
        {
            get { return _claimsIdentities; }
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
        public bool Equals(ClaimsPrincipal other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._identity, _identity);
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
            if (obj.GetType() != typeof (ClaimsPrincipal)) return false;
            return Equals((ClaimsPrincipal) obj);
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
            return (_identity != null ? _identity.GetHashCode() : 0);
        }

        public static bool operator ==(ClaimsPrincipal left, ClaimsPrincipal right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ClaimsPrincipal left, ClaimsPrincipal right)
        {
            return !Equals(left, right);
        }
        #endregion
    }
}
