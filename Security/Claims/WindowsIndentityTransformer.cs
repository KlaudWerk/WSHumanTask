using System;
using System.Security.Principal;
using Microsoft.IdentityModel.Claims;

namespace KlaudWerk.Security.Claims
{
    /// <summary>
    /// Transform Windows Identity to a Claims Indentity
    /// </summary>
    public class WindowsIndentityTransformer:IIdentityTransformer<WindowsIdentity>
    {
        private readonly IAccountFactory _accountFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsIndentityTransformer"/> class.
        /// </summary>
        /// <param name="accountFactory">The account factory.</param>
        public WindowsIndentityTransformer(IAccountFactory accountFactory)
        {
            _accountFactory = accountFactory;
        }

        /// <summary>
        /// Transforms the specified identity.
        /// </summary>
        /// <param name="identity">The identity.</param>
        /// <returns></returns>
        public IClaimsIdentity Transform(WindowsIdentity identity)
        {
            //WindowsPrincipal principal = new WindowsPrincipal(identity);
            //bool isLocalAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            //Guid mappedAccountId =
            //    _accountFactory.GetMappedAccountId(identity.AuthenticationType,identity.User == null
            //                                                                                  ? identity.Name
            //                                                                                  : identity.User.Value);
            //ClaimsIdentity claimsIdentity =
            //    new ClaimsIdentity(mappedAccountId)
            //        {
            //            IsAuthenticated = identity.IsAuthenticated
            //        };
            //claimsIdentity.Claims.Add(new Claim(ClaimType.Administrator.ToString(),isLocalAdmin?"true":"false"));
            //if (identity.Groups != null)
            //    foreach(IdentityReference reference in identity.Groups)
            //    {
            //        string groupSid = reference.Value;
            //        Account group =
            //            _accountFactory.GetAccount(new AccountId(identity.AuthenticationType, groupSid, null));
            //        claimsIdentity.Claims.Add(new Claim(ClaimType.Group.ToString(),group.Id.MappedId.ToString()));
            //    }
            //foreach (Account account in _accountFactory.GetRoles(mappedAccountId))
            //    claimsIdentity.Claims.Add(new Claim(ClaimType.Role.ToString(),account.Id.MappedId.ToString()));
            //foreach (Account account in _accountFactory.GetGroups(mappedAccountId))
            //    claimsIdentity.Claims.Add(new Claim(ClaimType.Role.ToString(), account.Id.MappedId.ToString()));
            //return claimsIdentity;
            return null;
        }
    }
}
