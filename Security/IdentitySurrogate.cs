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
using KlaudWerk.Security.Claims;
using Microsoft.IdentityModel.Claims;

namespace KlaudWerk.Security
{
    /// <summary>
    /// External identity surrogate
    /// </summary>
    public class IdentitySurrogate:IClaimsIdentity, 
        IEquatable<IdentitySurrogate>
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        public IdentityId Id { get; private set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="IdentitySurrogate"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        public IdentitySurrogate(IdentityId id)
        {
            Id = id;
            Claims = new ClaimCollection(this)
                                  {
                                      new Claim(ClaimType.MappedId.ToString(), Id.ToString())
                                  };
         }

        /// <summary>
        /// Gets the name of the current user.
        /// </summary>
        /// <returns>
        /// The name of the user on whose behalf the code is running.
        /// </returns>
        public string Name { get { return Id.ToString(); } }

        /// <summary>
        /// Gets the type of authentication used.
        /// </summary>
        /// <returns>
        /// The type of authentication used to identify the user.
        /// </returns>
        public string AuthenticationType { get { return "Surrogate"; } }

        /// <summary>
        /// Gets a value that indicates whether the user has been authenticated.
        /// </summary>
        /// <returns>
        /// true if the user was authenticated; otherwise, false.
        /// </returns>
        public bool IsAuthenticated { get; set; }

        /// <summary>
        /// Copies this instance.
        /// </summary>
        /// <returns></returns>
        public IClaimsIdentity Copy()
        {
            return new IdentitySurrogate(Id);
        }

        /// <summary>
        /// Gets the claims.
        /// </summary>
        /// <value>The claims.</value>
        public ClaimCollection Claims
        {
            get; 
            private set;
        }

        /// <summary>
        /// Gets or sets the actor.
        /// </summary>
        /// <value>The actor.</value>
        public IClaimsIdentity Actor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        /// <value>The label.</value>
        public string Label
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the type of the name claim.
        /// </summary>
        /// <value>The type of the name claim.</value>
        public string NameClaimType
        {
            get { return ClaimType.AccountName.ToString(); }
            set { }
        }

        /// <summary>
        /// Gets or sets the type of the role claim.
        /// </summary>
        /// <value>The type of the role claim.</value>
        public string RoleClaimType
        {
            get { return ClaimType.Role.ToString(); }
            set { }
        }

        /// <summary>
        /// Gets or sets the bootstrap token.
        /// </summary>
        /// <value>The bootstrap token.</value>
        public SecurityToken BootstrapToken
        {
            get;
            set;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.
        ///                 </param>
        public bool Equals(IdentitySurrogate other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Id, Id);
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
            if (obj.GetType() != typeof (IdentitySurrogate)) return false;
            return Equals((IdentitySurrogate) obj);
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
            return (Id != null ? Id.GetHashCode() : 0);
        }

        public static bool operator ==(IdentitySurrogate left, IdentitySurrogate right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(IdentitySurrogate left, IdentitySurrogate right)
        {
            return !Equals(left, right);
        }
   }
}
