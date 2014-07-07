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
using System.Security.Principal;
using HumanTask.Security;
using KlaudWerk.Security;

namespace HumanTask
{
    /// <summary>
    /// A store for Human Role assignments
    /// Can implement expirable caching, back-end database storage etc
    /// </summary>
    public interface IAssignmentService
    {
        /// <summary>
        /// Assigns the specified role.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <param name="identities">The identities.</param>
        void Assign(HumanRoles role, IEnumerable<IdentityId> identities);
        /// <summary>
        /// Assigns the specified role.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <param name="expression">The expression.</param>
        void Assign(HumanRoles role, string expression);
        /// <summary>
        /// Excludes specified identities from the asssignment.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <param name="identities">The identities.</param>
        void Exclude(HumanRoles role, IEnumerable<IdentityId> identities);
        /// <summary>
        /// Removes the assignment.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        void RemoveAssignment(HumanRoles role, string expression);
        /// <summary>
        /// Removes the assignment.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <param name="identities">The identities.</param>
        /// <returns></returns>
        void RemoveAssignment(HumanRoles role, IEnumerable<IdentityId> identities);
        /// <summary>
        /// Gets the assignments for a Human role.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <returns></returns>
        IEnumerable<IIdentity> GetAssignments(HumanRoles role);
    }
}