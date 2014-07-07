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
using System.Linq;
using Microsoft.IdentityModel.Claims;

namespace KlaudWerk.Security.Claims
{
    /// <summary>
    /// Extensions methods for Claims identity
    /// </summary>
    public static class ClaimsIdentityExt
    {
        /// <summary>
        /// Return all vaules of claims of specified type
        /// </summary>
        /// <param name="identity">The identity.</param>
        /// <param name="cType">type of a claim</param>
        /// <returns>values</returns>
        public static IEnumerable<string> GetValuesOfClaim(this IClaimsIdentity identity, string cType)
        {
            var c = from claim in identity.Claims
                    where claim.ClaimType == cType
                    select claim.Value;
            return c.AsEnumerable();
        }

    }
}
