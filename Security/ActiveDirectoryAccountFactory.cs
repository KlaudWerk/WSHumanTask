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
using System.DirectoryServices;

namespace KlaudWerk.Security
{
    /// <summary>
    /// Windows AD account factory
    /// </summary>
    public class ActiveDirectoryAccountFactory:AccountFactoryBase
    {

        private readonly DirectoryEntry _root;
        /// <summary>
        /// Initializes a new instance of the <see cref="ActiveDirectoryAccountFactory"/> class.
        /// </summary>
        /// <param name="ldapRoot">The LDAP root.</param>
        /// <param name="accountStorageInteraction">The account store.</param>
        public ActiveDirectoryAccountFactory(string ldapRoot,IAccountStorageInteraction accountStorageInteraction):base(accountStorageInteraction)
        {
            _root = new DirectoryEntry(ldapRoot);
        }

        #region Implementation of IAccountFactory

        /// <summary>
        /// Account FActory Authentication type
        /// </summary>
        /// <value>The authentication.</value>
        protected override string Authentication
        {
            get { return "Kerberos"; }
        }
        /// <summary>
        /// Retrieve account information from uderlaying authentication system
        /// </summary>
        /// <param name="nativeId">The native id.</param>
        /// <param name="accountName">Name of the account.</param>
        /// <param name="accountDisplayName">Display name of the account.</param>
        protected override void GetAccountInfo(string nativeId, out string accountName, out string accountDisplayName)
        {
            TryGetDistinguishedNameBySid(nativeId, out accountName, out accountDisplayName);
        }

        #endregion

        /// <summary>
        /// Search Active directoory by SID
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="dn"></param>
        /// <param name="displayname"></param>
        /// <returns></returns>
        protected bool TryGetDistinguishedNameBySid(string sid, out string dn, out string displayname)
        {
            dn = null;
            displayname = null;
            bool rc = false;
            DirectorySearcher searcher = new DirectorySearcher(_root)
            {
                Asynchronous = true,
                CacheResults = true,
                Filter = string.Format("(objectSid={0})", sid)
            };
            searcher.PropertiesToLoad.AddRange(
                new[]
                    {
                    "distinguishedName",
                    "displayName",
                    "name"
                    }
                );
            SearchResult result = searcher.FindOne();
            if (result != null)
            {

                ResultPropertyValueCollection values = result.Properties["distinguishedName"];
                if (values.Count != 0)
                {
                    dn = values[0].ToString();
                    rc = true;
                }
                values = result.Properties["displayName"];
                if (values.Count != 0)
                {
                    displayname = values[0].ToString();
                }
                else
                {
                    values = result.Properties["name"];
                    if (values.Count != 0)
                    {
                        displayname = values[0].ToString();
                    }
                }
            }
            return rc;
        }

    }
}
