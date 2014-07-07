using System;
using System.Linq;
using System.Security.Principal;
using HumanTask.Security;

namespace HumanTask
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
        public static bool IsInAnyRole(this IPrincipal principal, params HumanRoles[] roles)
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
        public static bool IsInAllRoles(this IPrincipal principal, params HumanRoles[] roles)
        {
            return roles.Aggregate(true, (current, role) => current & principal.IsInRole(role));
        }

        public static bool IsGroupIdentity(this IIdentity identity)
        {
            return identity is IGroupIdentity;
        }

        public static IdentityId  GetMappedId(this IIdentity identity)
        {
            return new IdentityId(Guid.NewGuid());
        }

        public static IIdentity GetIdentity(this IdentityId id)
        {
            return  id==null?null:new IdentitySurrogate(id, string.Empty, string.Empty, "local", false, false);
        }

    }

    public interface IGroupIdentity:IIdentity
    {
    }
}