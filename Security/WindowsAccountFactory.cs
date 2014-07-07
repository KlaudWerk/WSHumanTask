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
using System.Text;

namespace KlaudWerk.Security
{
    /// <summary>
    /// Windows Accounts without ActiveDirecory
    /// </summary>
    public class WindowsAccountFactory: AccountFactoryBase
    {
        private readonly string _authType;

        #region Implementation of IAccountFactory

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsAccountFactory"/> class.
        /// </summary>
        /// <param name="authType">Type of the auth.</param>
        /// <param name="accountStorageInteraction">The account store.</param>
        public WindowsAccountFactory(string authType,
            IAccountStorageInteraction accountStorageInteraction) :
            base(accountStorageInteraction)
        {
            _authType = authType;
        }

        /// <summary>
        /// Account FActory Authentication type
        /// </summary>
        /// <value>The authentication.</value>
        protected override string Authentication
        {
            get { return _authType; }
        }

        /// <summary>
        /// Retrieve account information from uderlaying authentication system
        /// </summary>
        /// <param name="nativeId">The native id.</param>
        /// <param name="accountName">Name of the account.</param>
        /// <param name="accountDisplayName">Display name of the account.</param>
        protected override void GetAccountInfo(string nativeId, out string accountName, out string accountDisplayName)
        {
            SecurityIdentifier sid = new SecurityIdentifier(nativeId);
            IdentityReference idRef = sid.Translate(typeof(NTAccount));
            accountDisplayName = accountName = idRef.Value;
        }
    }
}
