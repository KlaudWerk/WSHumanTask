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
using System.Reflection;
using System.Linq;
using System.Security.Principal;
using log4net;
using Microsoft.IdentityModel.Claims;

namespace KlaudWerk.Security.Claims
{
    /// <summary>
    /// Transform Windows Identity to a Claims Indentity
    /// </summary>
    public class WindowsIdentityTransformer:IIdentityTransformer<WindowsIdentity>
    {
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IAccountFactory _accountFactory;
        private static readonly Dictionary<string,string> _wellKnownSidValues=new Dictionary<string,string>();

        static WindowsIdentityTransformer()
        {
            for(int i=0; i<(int)WellKnownSidType.MaxDefined; i++)
            {
                WellKnownSidType sidType = (WellKnownSidType) i;
                try
                {
                    SecurityIdentifier sid = new SecurityIdentifier(sidType, null);
                    _wellKnownSidValues[sidType.ToString()] = sid.Value;
                }
                catch
                {
                    if(_logger.IsDebugEnabled)
                        _logger.Debug(string.Format("Cannot create SID for {0}",sidType));
                }
            }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsIdentityTransformer"/> class.
        /// </summary>
        /// <param name="accountFactory">The account factory.</param>
        public WindowsIdentityTransformer(IAccountFactory accountFactory)
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
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            bool isLocalAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            IdentityId mappedAccountId =
                _accountFactory.GetMappedAccountId(identity.AuthenticationType, identity.User == null
                                                                                              ? identity.Name
                                                                                              : identity.User.Value);
            ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(mappedAccountId)
                    {
                        IsAuthenticated = identity.IsAuthenticated
                    };
            claimsIdentity.Claims.Add(new Claim(ClaimType.Administrator.ToString(), isLocalAdmin ? "true" : "false"));
            claimsIdentity.Claims.Add(new Claim(ClaimType.Authentication.ToString(), identity.AuthenticationType));
            if (identity.Groups != null)
            {
                foreach (IdentityReference reference in identity.Groups)
                {
                    string groupSid = reference.Value;
                    if (IsWellKnownSid(groupSid))
                        continue;
                    IdentityId groupId =
                        _accountFactory.GetMappedAccountId(identity.AuthenticationType, groupSid);
                    claimsIdentity.Claims.Add(new Claim(ClaimType.Group.ToString(), groupId.ToString()));
                }
            }
            foreach (Account account in _accountFactory.GetRoles(mappedAccountId))
                claimsIdentity.Claims.Add(new Claim(ClaimType.Role.ToString(), account.MappedId.ToString()));
            foreach (Account account in _accountFactory.GetGroups(mappedAccountId))
                claimsIdentity.Claims.Add(new Claim(ClaimType.Role.ToString(), account.MappedId.ToString()));
            return claimsIdentity;
        }

        /// <summary>
        /// Determines whether the SID is one of the well-known SIDs
        /// </summary>
        /// <param name="sid">The group sid.</param>
        /// <returns>
        ///   <c>true</c> if is well - known sid; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsWellKnownSid(string sid)
        {
            return _wellKnownSidValues.Values.Contains(sid);
        }
    }
}
