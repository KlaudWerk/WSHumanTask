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
using System.Collections.Generic;

namespace KlaudWerk.Security
{
    /// <summary>
    /// Account Store interfce
    /// </summary>
    public interface IAccountStorageInteraction
    {
        /// <summary>
        /// Maps the account.
        /// </summary>
        /// <param name="authType">Type of the auth.</param>
        /// <param name="nativeId">The native id.</param>
        /// <param name="accountName">Name of the account.</param>
        /// <param name="displayName">The display name.</param>
        /// <returns></returns>
        IdentityId MapAccount(string authType, string nativeId, string accountName, string displayName);
        /// <summary>
        /// Gets the mapped account.
        /// </summary>
        /// <param name="authType">Type of the auth.</param>
        /// <param name="nativeId">The native id.</param>
        /// <returns></returns>
        IdentityId GetMappedAccount(string authType, string nativeId);
        /// <summary>
        /// Gets the list of roles for the account.
        /// </summary>
        /// <param name="mappedId">The mapped id.</param>
        /// <returns></returns>
        IEnumerable<Account> GetRoles(IdentityId mappedId);
        /// <summary>
        /// Gets the flat list of groups in which the account has membership.
        /// </summary>
        /// <param name="mappedId">The mapped id.</param>
        /// <returns></returns>
        IEnumerable<Account> GetGroups(IdentityId mappedId);
    }
}
