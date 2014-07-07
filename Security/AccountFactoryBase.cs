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
using System.Collections.Generic;


namespace KlaudWerk.Security
{
    /// <summary>
    /// Base class for Account Factories
    /// </summary>
    public abstract class AccountFactoryBase : IAccountFactory
    {
        /// <summary>
        /// Account FActory Authentication type
        /// </summary>
        /// <value>The authentication.</value>
        protected abstract string Authentication { get;  }

        private readonly IAccountStorageInteraction _accountStorageInteraction;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountFactoryBase"/> class.
        /// </summary>
        /// <param name="accountStorageInteraction">The account store.</param>
        protected AccountFactoryBase(IAccountStorageInteraction accountStorageInteraction)
        {
            _accountStorageInteraction = accountStorageInteraction;
        }

        /// <summary>
        /// Gets the mapped account id.
        /// </summary>
        /// <param name="authenticationType">Type of the authentication.</param>
        /// <param name="nativeId">The native id.</param>
        /// <returns></returns>
        public virtual IdentityId GetMappedAccountId(string authenticationType, string nativeId)
        {
            if (!Authentication.Equals(authenticationType, StringComparison.InvariantCultureIgnoreCase))
                throw new ArgumentException(string.Format("Unsupported authentication type:{0}", authenticationType));
            IdentityId identity;
            if (TryGetMappedAccount(nativeId, out identity))
                return identity;
            string accountName, accountDisplayName;
            GetAccountInfo(nativeId,out accountName,out accountDisplayName);
            return _accountStorageInteraction.MapAccount(Authentication, nativeId, accountName, accountDisplayName);
        }

        public virtual Account GetAccount(IdentityId id)
        {
            throw new NotImplementedException();
        }

        public virtual IEnumerable<Account> GetRoles(IdentityId id)
        {
            return new Account[] {};
        }

        public virtual  IEnumerable<Account> GetGroups(IdentityId id)
        {
            return new Account[] {};
        }

        /// <summary>
        /// Trying to retrieve account mapped Id
        /// </summary>
        /// <param name="nativeId">The native id.</param>
        /// <param name="mapped">The mapped.</param>
        /// <returns></returns>
        protected bool TryGetMappedAccount(string nativeId,out IdentityId mapped)
        {
            return (mapped = _accountStorageInteraction.GetMappedAccount(Authentication, nativeId)) != null;
        }

        /// <summary>
        /// Retrieve account information from uderlaying authentication system
        /// </summary>
        /// <param name="nativeId">The native id.</param>
        /// <param name="accountName">Name of the account.</param>
        /// <param name="accountDisplayName">Display name of the account.</param>
        protected abstract void GetAccountInfo(string nativeId,out string accountName,out string accountDisplayName);
        
    }
}