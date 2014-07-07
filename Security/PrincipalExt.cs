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
using System.Linq;
using System.Security.Principal;
using Microsoft.IdentityModel.Claims;
using KlaudWerk.Security.Claims;
using Microsoft.Practices.ServiceLocation;

namespace KlaudWerk.Security
{
    /// <summary>
    /// Extensions for IPrincipal 
    /// </summary>
    public static class PrincipalExt
    {
        /// <summary>
        /// Determines whether the specified principal in multiple roles.
        /// </summary>
        /// <param name="principal">The principal.</param>
        /// <param name="roles">The roles.</param>
        /// <returns>
        /// 	<c>true</c> if [is in roles] [the specified principal]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsInAnyRole(this IPrincipal principal, params string[] roles)
        {
            return roles.Aggregate(false, (current, role) => current | principal.IsInRole(role));
        }
        /// <summary>
        /// Determines whether the specified principal in all roles.
        /// </summary>
        /// <param name="principal">The principal.</param>
        /// <param name="roles">The roles.</param>
        /// <returns>
        /// 	<c>true</c> if [is in roles] [the specified principal]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsInAllRoles(this IPrincipal principal, params string[] roles)
        {
            return roles.Aggregate(true, (current, role) => current & principal.IsInRole(role));
        }

        /// <summary>
        /// Determines whether the identity is a member of the accounts collection,
        /// either directly, or indirectly through a group or a role membership
        /// </summary>
        /// <param name="accounts">The accounts.</param>
        /// <param name="identity">The identity.</param>
        /// <returns>
        /// 	<c>true</c> if the identity is a member of the collection ; otherwise, <c>false</c>.
        /// </returns>
        public static bool ContainsOrMemberOf(this ICollection<IIdentity> accounts,IIdentity identity)
        {
            if (accounts.Count(a => a.GetMappedId() == identity.GetMappedId()) > 0)
                return true;
            IEnumerable<IIdentity> groups =identity.MemberOf();
            IEnumerable<IIdentity> roles = identity.InRoles();
            return 
                groups.Select(a=>a.GetMappedId()).Any(accounts.Select(a=>a.GetMappedId()).Contains) ||
                roles.Select(a => a.GetMappedId()).Any(accounts.Select(a => a.GetMappedId()).Contains);
        }

        /// <summary>
        /// Get list of roles, in which this identity included as a member
        /// </summary>
        /// <param name="identity">The identity.</param>
        /// <returns></returns>
        public static IEnumerable<IIdentity> MemberOf(this IIdentity identity)
        {
            IClaimsIdentity claimsIdentity = identity as IClaimsIdentity;
            if(claimsIdentity==null)
                return new List<IIdentity>();
            if (string.Equals(claimsIdentity.AuthenticationType,"Surrogate"))
            {
                IEnumerable<Account> groups = ServiceLocator.Current.GetInstance<IAccountFactory>().
                    GetGroups(identity.GetMappedId());
                return groups.Select(a => new IdentitySurrogate(a.MappedId));
            }
            return claimsIdentity.GetValuesOfClaim(ClaimType.Group.ToString()).Select(
                s => new IdentitySurrogate(new IdentityId(s)));
        }

        /// <summary>
        /// Get the list of roles for specific identity
        /// </summary>
        /// <param name="identity">The identity.</param>
        /// <returns></returns>
        public static IEnumerable<IIdentity> InRoles(this IIdentity identity)
        {
            IClaimsIdentity claimsIdentity = identity as IClaimsIdentity;
            if (claimsIdentity == null)
                return new List<IIdentity>();
            if(string.Equals(claimsIdentity.AuthenticationType,"Surrogate"))
            {
                IEnumerable<Account> roles = ServiceLocator.Current.GetInstance<IAccountFactory>().
                    GetRoles(identity.GetMappedId());
                return roles.Select(a => new IdentitySurrogate(a.MappedId));
            }
            return claimsIdentity.GetValuesOfClaim(ClaimType.Role.ToString()).Select(
                s => new IdentitySurrogate(new IdentityId(s)));
        }

        /// <summary>
        /// Determines whether the specified identity is a group or role identity.
        /// </summary>
        /// <param name="identity">The identity.</param>
        /// <returns>
        /// 	<c>true</c> if [is group identity] [the specified identity]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsGroupIdentity(this IIdentity identity)
        {
            IClaimsIdentity claimsIdentity = identity as IClaimsIdentity;
            if (claimsIdentity == null)
                return false;
            string accType = claimsIdentity.GetValuesOfClaim(ClaimType.AccountType.ToString()).FirstOrDefault();
            if (string.IsNullOrEmpty(accType) && string.Equals(claimsIdentity.AuthenticationType,"Surrogate"))
            {
                Account account = ServiceLocator.Current.GetInstance<IAccountFactory>().GetAccount(identity.GetMappedId());
                claimsIdentity.Claims.Add(new Claim(ClaimType.AccountType.ToString(), accType = account.AccountType.ToString()));
            }
            return ClaimValue.GroupAccountType.Equals(accType, 
                StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Gets the mapped id from the Claims Identity.
        /// </summary>
        /// <param name="identity">The identity.</param>
        /// <returns></returns>
        public static IdentityId  GetMappedId(this IIdentity identity)
        {
            if (identity == null)
                return null;
            IClaimsIdentity claimsIdentity = identity as IClaimsIdentity;
            if(claimsIdentity==null)
                throw new ArgumentException();
            string mappedId=claimsIdentity.GetValuesOfClaim(ClaimType.MappedId.ToString()).Single();
            return new IdentityId(mappedId);
        }

        /// <summary>
        /// Gets the identity.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public static IClaimsIdentity GetIdentity(this IdentityId id)
        {
            return id == null ? null : new IdentitySurrogate(id);
        }
    }
}