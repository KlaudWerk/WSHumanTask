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
using HumanTask.Security;
using KlaudWerk.Security;

namespace HumanTask
{
    /// <summary>
    /// Human role assignment
    /// </summary>
    public class RoleAssignmentEntity
    {
        public virtual int Id { get; set; }
        /// <summary>
        /// Gets or sets the identities.
        /// </summary>
        /// <value>
        /// The identities that included into the assignment.
        /// </value>
        public virtual ICollection<IdentityId> PotentialOwners { get; set; }
        /// <summary>
        /// Gets or sets the excluded identities.
        /// </summary>
        /// <value>
        /// The identities that excluded from the assignment.
        /// </value>
        public virtual ICollection<IdentityId> ExcludedOwners { get; set; }
        /// <summary>
        /// Gets or sets the business administrators.
        /// </summary>
        /// <value>The business administrators.</value>
        public virtual ICollection<IdentityId> BusinessAdministrators { get; set; }
        /// <summary>
        /// Gets or sets the stakeholders.
        /// </summary>
        /// <value>The stakeholders.</value>
        public virtual ICollection<IdentityId> Stakeholders { get; set; }
        /// <summary>
        /// Gets or sets the recepients.
        /// </summary>
        /// <value>The recepients.</value>
        public virtual ICollection<IdentityId> Recepients { get; set; }
        /// <summary>
        /// Gets or sets the potential delegatees.
        /// </summary>
        /// <value>The potential delegatees.</value>
        public virtual ICollection<IdentityId> PotentialDelegatees { get; set; }
        /// <summary>
        /// Gets or sets the identities select expression.
        /// </summary>
        /// <value>
        /// The expression that fetch indentities included into the assignment
        /// </value>
        public virtual string IdentitiesSelectExpression { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleAssignmentEntity"/> class.
        /// </summary>
        public RoleAssignmentEntity()
        {
            PotentialOwners = new List<IdentityId>();
            ExcludedOwners=new List<IdentityId>();
            BusinessAdministrators = new List<IdentityId>();
            Stakeholders = new List<IdentityId>();
            Recepients = new List<IdentityId>();
            PotentialDelegatees = new List<IdentityId>();

        }
    }
}