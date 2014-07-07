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
namespace KlaudWerk.Security
{
    /// <summary>
    /// Account entity
    /// </summary>
    public class Account
    {

        /// <summary>
        /// Gets the mapped id.
        /// </summary>
        /// <value>The mapped id.</value>
        public virtual IdentityId MappedId
        {
            get; set;
        }

        /// <summary>
        /// Gets the authentication system.
        /// </summary>
        /// <value>The auth system.</value>
        public virtual string AuthSystem
        {
            get; set;
        }

        /// <summary>
        /// Gets the Account's Native Id
        /// </summary>
        /// <value>The auth system id.</value>
        public virtual string NativeId
        {
            get; set;
        }

        /// <summary>
        /// Gets the name of the distnguished.
        /// </summary>
        /// <value>The name of the distnguished.</value>
        public virtual string DistinguishedName
        {
            get; set;
        }

        /// <summary>
        /// Gets the display name.
        /// </summary>
        /// <value>The display name.</value>
        public virtual string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the type of the account.
        /// </summary>
        /// <value>The type of the account.</value>
        public virtual AccountType AccountType { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Account"/> class.
        /// </summary>
        public Account()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Account"/> class.
        /// </summary>
        /// <param name="mappedId">The mapped id.</param>
        /// <param name="nativeId">The native id.</param>
        /// <param name="authSystem">The auth system.</param>
        /// <param name="accountType">Type of the account.</param>
        public Account(IdentityId mappedId,string nativeId,string authSystem,AccountType accountType)
        {
            MappedId = mappedId;
            NativeId = nativeId;
            AuthSystem = authSystem;
            AccountType = accountType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Account"/> class.
        /// </summary>
        /// <param name="mappedId">The mapped id.</param>
        /// <param name="nativeId">The native id.</param>
        /// <param name="authSystem">The auth system.</param>
        /// <param name="accountType">Type of the account.</param>
        /// <param name="distinguishedName">Name of the distinguished.</param>
        /// <param name="displayName">The display name.</param>
        public Account(IdentityId mappedId, string nativeId, string authSystem, AccountType accountType, 
            string distinguishedName, string displayName) :
            this(mappedId,nativeId,authSystem,accountType)
        {
            DistinguishedName = distinguishedName;
            DisplayName = displayName;
        }
    }
}
