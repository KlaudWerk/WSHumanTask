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
using System.Security.Principal;
using KlaudWerk.Security;

namespace HumanTask.Security
{
    /// <summary>
    /// The Task Principal Wrapper class
    /// </summary>
    internal class TaskPrincipal:IPrincipal, IEquatable<TaskPrincipal>
    {
        private readonly TaskEntity _task;
        private readonly IPrincipal _principal;

        private static readonly Dictionary<string, Func<TaskPrincipal, bool>>
            _hrMap = new Dictionary<string, Func<TaskPrincipal, bool>>
                       {
                         { HumanRoles.ActualOwner,tp=>tp._task.ActualOwner.GetMappedId()==tp.Identity.GetMappedId() },  
                         { HumanRoles.BusinessAdministrator,tp=>tp._task.BusinessAdministrators.ContainsOrMemberOf(tp.Identity)},  
                         { HumanRoles.ExcludedOwner,tp=>tp._task.ExcludedOwners.ContainsOrMemberOf(tp.Identity) },  
                         { HumanRoles.Initiator,tp=>tp._task.Initiator.GetMappedId()==tp.Identity.GetMappedId() },  
                         { HumanRoles.PotentialDelegatee,tp=>tp._task.PotentialDelegatees.ContainsOrMemberOf(tp.Identity) },  
                         { HumanRoles.PotentialOwner,tp=>
                             tp._task.PotentialOwners.ContainsOrMemberOf(tp.Identity) &&
                             !tp._task.ExcludedOwners.ContainsOrMemberOf(tp.Identity)
                             },  
                         { HumanRoles.Recepient,tp=>tp._task.Recepients.ContainsOrMemberOf(tp.Identity) },  
                         { HumanRoles.Stakeholder,tp=>tp._task.Stakeholders.ContainsOrMemberOf(tp.Identity) }  
                       };

        /// <summary>
        /// Gets the task.
        /// </summary>
        /// <value>The task.</value>
        public TaskEntity Task
        {
            get { return _task; }
        }

        /// <summary>
        /// Gets the principal.
        /// </summary>
        /// <value>The principal.</value>
        public IPrincipal Principal
        {
            get { return _principal; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskPrincipal"/> class.
        /// </summary>
        /// <param name="principal">The principal.</param>
        /// <param name="task">The task.</param>
        public TaskPrincipal(IPrincipal principal, TaskEntity task)
        {
            _task = task;
            _principal = principal;
        }

        /// <summary>
        /// Determines whether the current principal belongs to the specified role.
        /// </summary>
        /// <returns>
        /// true if the current principal is a member of the specified role; otherwise, false.
        /// </returns>
        /// <param name="role">The name of the role for which to check membership. 
        ///                 </param>
        public bool IsInRole(string role)
        {
            Func<TaskPrincipal, bool> f;
            return _hrMap.TryGetValue(role, out f) ? f.Invoke(this) : _principal.IsInRole(role);
        }

        /// <summary>
        /// Gets the identity of the current principal.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Security.Principal.IIdentity"/> object associated with the current principal.
        /// </returns>
        public IIdentity Identity
        {
            get { return _principal.Identity; }
        }

        #region IEquatable
        public bool Equals(TaskPrincipal other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._principal, _principal);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (TaskPrincipal)) return false;
            return Equals((TaskPrincipal) obj);
        }

        public override int GetHashCode()
        {
            return (_principal != null ? _principal.GetHashCode() : 0);
        }

        public static bool operator ==(TaskPrincipal left, TaskPrincipal right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(TaskPrincipal left, TaskPrincipal right)
        {
            return !Equals(left, right);
        }
        #endregion
    }
}
