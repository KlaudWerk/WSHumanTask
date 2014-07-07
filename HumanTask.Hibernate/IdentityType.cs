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
using System.Security.Principal;
using HumanTask.Hibernate.Helpers;
using KlaudWerk.Security;

namespace HumanTask.Hibernate
{
    /// <summary>
    /// Custom User Type for IdentityId
    /// </summary>
    public class IdentityType:GuidIdType<IIdentity>
    {
        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="val">The val.</param>
        /// <returns></returns>
        protected override Guid? GetValue(object val)
        {
            if (val == null)
                return null;
            IIdentity identity = val as IIdentity;
            if (identity == null)
                throw new ArgumentException("Unexpected type:{0}",val.GetType().FullName);
            return identity.GetMappedId().Id;
        }

        /// <summary>
        /// Insts the value.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <returns></returns>
        protected override IIdentity InstValue(object o)
        {
            
            return o==null?null:(new IdentityId((Guid)o)).GetIdentity();
        }

    }
}