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
using HumanTask.Security;

namespace HumanTask
{
    /// <summary>
    /// The Task Entity
    /// </summary>
    public class TaskEntity
    {

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        public virtual int Id { get; set; }

        /// <summary>
        /// Version for optimistic locking
        /// </summary>
        public virtual int Version { get; set; }

        /// <summary>
        /// Gets the "Stable" task id.
        /// </summary>
        public virtual TaskId TaskId { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public virtual TaskStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        /// <value>The subject.</value>
        public virtual string Subject { get; set; }

        /// <summary>
        /// Gets or sets the priority.
        /// Can be executed by an Actual Owner, or a Business Administrator
        /// </summary>
        /// <exception cref="TaskAccessException"></exception>
        /// <value>
        /// The priority.
        /// </value>
        public virtual Priority Priority { get; set; }

        /// <summary>
        /// Gets a value indicating whether this task can be skipped
        /// Can be executed by a Business Administrator
        /// </summary>
        /// <exception cref="TaskAccessException"></exception>
        /// <value>
        /// 	<c>true</c> if this instance is skippable; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsSkippable { get; set; }

        /// <summary>
        /// Gets the task created date.
        /// </summary>
        public virtual DateTime Created { get; set; }

        /// <summary>
        /// Gets the time when the task instance was started.
        /// </summary>
        public virtual DateTime? Started { get; set; }

        /// <summary>
        /// Gets the time when the task instance was completed completed.
        /// </summary>
        public virtual DateTime? Completed { get; set; }

        /// <summary>
        /// Gets the task initiator.
        /// </summary>
        public virtual IIdentity Initiator { get; set; }

        /// <summary>
        /// Gets or sets the task's actual owner.
        /// </summary>
        /// <value>
        /// The actual owner.
        /// </value>
        public virtual IIdentity ActualOwner { get; set; }

        /// <summary>
        /// Gets the comments.
        /// </summary>
        public virtual ICollection<TaskComment> Comments { get; set; } 

        /// <summary>
        /// Gets the parent task.
        /// </summary>
        /// <value>The parent.</value>
        public virtual TaskEntity Parent { get; set; }

        /// <summary>
        /// Gets the subtasks.
        /// </summary>
        /// <value>The subtasks.</value>
        public virtual IList<TaskEntity> Subtasks { get; set; }

        /// <summary>
        /// Gets the task outcome.
        /// </summary>
        /// <value>The task outcome.</value>
        public virtual string TaskOutcome { get; set; }

        /// <summary>
        /// Gets or sets the state of the suspended.
        /// </summary>
        /// <value>The state of the suspended.</value>
        public virtual SuspendedState SuspendedState { get; set; }
        #region Identity Assignments
        /// <summary>
        /// Gets or sets the identities.
        /// </summary>
        /// <value>
        /// The identities that included into the assignment.
        /// </value>
        public virtual ICollection<IIdentity> PotentialOwners { get; set; }
        /// <summary>
        /// Gets or sets the excluded identities.
        /// </summary>
        /// <value>
        /// The identities that excluded from the assignment.
        /// </value>
        public virtual ICollection<IIdentity> ExcludedOwners { get; set; }
        /// <summary>
        /// Gets or sets the business administrators.
        /// </summary>
        /// <value>The business administrators.</value>
        public virtual ICollection<IIdentity> BusinessAdministrators { get; set; }
        /// <summary>
        /// Gets or sets the stakeholders.
        /// </summary>
        /// <value>The stakeholders.</value>
        public virtual ICollection<IIdentity> Stakeholders { get; set; }
        /// <summary>
        /// Gets or sets the recepients.
        /// </summary>
        /// <value>The recepients.</value>
        public virtual ICollection<IIdentity> Recepients { get; set; }
        /// <summary>
        /// Gets or sets the potential delegatees.
        /// </summary>
        /// <value>The potential delegatees.</value>
        public virtual ICollection<IIdentity> PotentialDelegatees { get; set; }
        #endregion
        /// <summary>
        /// Initializes a new instance of the <see cref="TaskEntity"/> class.
        /// </summary>
        public TaskEntity()
        {
            Comments = new List<TaskComment>();
            Subtasks = new List<TaskEntity>();
            PotentialOwners = new List<IIdentity>();
            ExcludedOwners = new List<IIdentity>();
            BusinessAdministrators = new List<IIdentity>();
            Stakeholders = new List<IIdentity>();
            Recepients = new List<IIdentity>();
            PotentialDelegatees=new List<IIdentity>();
        }
    }
}

