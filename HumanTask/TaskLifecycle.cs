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
using KlaudWerk.Security;

namespace HumanTask
{
    public  class TaskLifecycle
    {
        /// <summary>
        /// Claim responsibility for a task. The task to status will be set to 'Reserved'
        /// The claimant must be in the list of Potential Owners or Business Administrators.
        /// The Actual owner will be set to Claimant
        /// </summary>
        /// <param name="task">The task.</param>
        /// <param name="principal">The principal.</param>
        /// <exception cref="TaskAccessException"/>
        /// <exception cref="TaskInvalidStateException"/>
        public virtual void Claim(TaskEntity task, IPrincipal principal)
        {
            if (task.Status != TaskStatus.Ready)
                throw new TaskInvalidStateException();

            if (principal.IsInAnyRole(HumanRoles.PotentialOwner, HumanRoles.BusinessAdministrator))
            {
                task.Status = TaskStatus.Reserved;
                task.ActualOwner = principal.Identity;
            }
            else
            {
                throw new TaskAccessException();
            }
        }

        /// <summary>
        /// Starts the task, The task status will be set to 'InProgress'.
        /// This action can be taken by an Actual Owner or a Potential Owner, if the task is in 'Ready' state.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <param name="principal">The principal.</param>
        /// <exception cref="TaskAccessException"/>
        /// <exception cref="TaskInvalidStateException"/>
        public virtual void Start(TaskEntity task, IPrincipal principal)
        {
            switch (task.Status)
            {
                case TaskStatus.Reserved:
                    if (!principal.IsInRole(HumanRoles.ActualOwner))
                        throw new TaskAccessException();
                    break;
                case TaskStatus.Ready:
                    if (!principal.IsInRole(HumanRoles.PotentialOwner))
                        throw new TaskAccessException();
                    task.ActualOwner = principal.Identity;
                    break;
                default:
                    throw new TaskInvalidStateException();
            }
            task.Status = TaskStatus.InProgress;
            task.Started = DateTime.UtcNow;
        }

        /// <summary>
        /// Stops the task. Cancel/stop the processing of the task. The task returns to the 'Reserved' state.
        /// This action can be performed by an Actual Owner or a Business Administrator
        /// <exception cref="TaskAccessException"/>
        /// <exception cref="TaskInvalidStateException"/>
        /// </summary>
        public virtual void Stop(TaskEntity task, IPrincipal principal)
        {
            if (task.Status != TaskStatus.InProgress)
                throw new TaskInvalidStateException();
            if (!principal.IsInAnyRole(HumanRoles.ActualOwner, HumanRoles.BusinessAdministrator))
                throw new TaskAccessException();
            task.Status = TaskStatus.Reserved;
        }

        /// <summary>
        /// Releases the task, The task status will be set back 'Ready'.
        /// This action can be performed by an Actual Owner or a Business Administrator
        /// <exception cref="TaskAccessException"/>
        /// 	<exception cref="TaskInvalidStateException"/>
        /// </summary>
        /// <param name="task">The task.</param>
        /// <param name="principal">The principal.</param>
        public virtual void Release(TaskEntity task, IPrincipal principal)
        {
            if (task.Status != TaskStatus.Reserved)
                throw new TaskInvalidStateException();
            if (!principal.IsInAnyRole(HumanRoles.ActualOwner, HumanRoles.BusinessAdministrator))
                throw new TaskAccessException();
            task.Status = TaskStatus.Ready;
            task.ActualOwner = null;
        }

        ///	<summary>
        /// Suspends the task. The task status will be set as 'Suspended'
        /// This action can be performed by  Potential Owners(only if the task in 'Ready' state),
        /// an Actual Owner, or a Business Administrator
        /// </summary>
        /// 	<exception cref="TaskAccessException"/>
        /// 	<exception cref="TaskInvalidStateException"/>
        /// <param name="task">The task.</param>
        /// <param name="principal">The principal.</param>
        public virtual void Suspend(TaskEntity task, IPrincipal principal)
        {
            SuspendUntil(task,DateTime.MaxValue,principal);
        }

        /// <summary>
        /// Suspend the task until to a given date.
        /// This action can be performed by  Potential Owners(only if the task in 'Ready' state),
        /// an Actual Owner, or a Business Administrator
        /// </summary>
        /// <param name="task">The Task Entity</param>
        /// <param name="nextTime">The next time.</param>
        /// <param name="principal">The principal.</param>
        /// <exception cref="TaskAccessException"/>
        /// <exception cref="TaskInvalidStateException"/>
        public virtual void SuspendUntil(TaskEntity task, DateTime nextTime, IPrincipal principal)
        {
            switch (task.Status)
            {
                case TaskStatus.Ready:
                    if (!principal.IsInAnyRole(HumanRoles.ActualOwner, HumanRoles.PotentialOwner, HumanRoles.BusinessAdministrator))
                        throw new TaskAccessException();
                    break;
                case TaskStatus.Reserved:
                    if (!principal.IsInAnyRole(HumanRoles.ActualOwner, HumanRoles.BusinessAdministrator))
                        throw new TaskAccessException();
                    break;
                case TaskStatus.InProgress:
                    if (!principal.IsInAnyRole(HumanRoles.ActualOwner, HumanRoles.BusinessAdministrator))
                        throw new TaskAccessException();
                    break;
                default:
                    throw new TaskInvalidStateException();
            }
            TaskStatus oldStatus = task.Status;
            task.Status = TaskStatus.Suspended;
            task.SuspendedState = new SuspendedState
            {
                OriginalOwner = task.ActualOwner.GetMappedId(),
                OriginalStatus = oldStatus,
                OperationPerformed = DateTime.UtcNow,
                SuspensionEnds = nextTime
            };
        }

        /// <summary>
        /// Suspend the task for a given period of time.
        /// This action can be performed by Potential Owners(only if the task in 'Ready' state),
        /// an Actual Owner, or a Business Administrator
        /// </summary>
        /// <param name="task">The task entity</param>
        /// <param name="timePeriod">The time period.</param>
        /// <param name="principal">The principal.</param>
        /// <exception cref="TaskAccessException"/>
        /// <exception cref="TaskInvalidStateException"/>
        public virtual void SuspendUntil(TaskEntity task, TimeSpan timePeriod, IPrincipal principal)
        {
            SuspendUntil(task,DateTime.UtcNow+timePeriod,principal);
        }

        /// <summary>
        /// Resumes the suspended task.
        /// The task status will be set to the value that the task had before 'Suspend' action was called.
        /// This action can be performed by  Potential Owners(only if the task in 'Ready' state),
        /// an Actual Owner, or a Business Administrator
        /// </summary>
        /// <param name="task">The task.</param>
        /// <param name="principal">The principal.</param>
        /// <exception cref="TaskAccessException"/>
        /// <exception cref="TaskInvalidStateException"/>
        public virtual void Resume(TaskEntity task, IPrincipal principal)
        {
            if (task.Status != TaskStatus.Suspended || task.SuspendedState == null)
                throw new TaskInvalidStateException();
            if (principal.IsInAnyRole(HumanRoles.ActualOwner, HumanRoles.BusinessAdministrator) ||
                ((principal.IsInRole(HumanRoles.PotentialOwner) && task.SuspendedState.OriginalStatus == TaskStatus.Ready)))
            {
                task.Status = task.SuspendedState.OriginalStatus;
                task.SuspendedState = null;
            }
            else
            {
                throw new TaskAccessException();
            }
        }

        /// <summary>
        /// Completes the task successfully finished.
        /// This action can only be performed by an Actual Owner.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <param name="principal">The principal.</param>
        /// <exception cref="TaskAccessException"/>
        /// <exception cref="TaskInvalidStateException"/>
        public virtual void Complete(TaskEntity task, IPrincipal principal)
        {
            if (task.Status != TaskStatus.InProgress)
                throw new TaskInvalidStateException();
            if (!principal.IsInRole(HumanRoles.ActualOwner))
                throw new TaskAccessException();
            task.Status = TaskStatus.Completed;
            task.Completed = DateTime.UtcNow;
        }

        /// <summary>
        /// Fails the task.
        /// Completes the execution of the task by raising a fault.
        /// This action can only be performed by an Actual Owner.
        /// <exception cref="TaskAccessException"/>
        /// 	<exception cref="TaskInvalidStateException"/>
        /// </summary>
        /// <param name="task">The task entity</param>
        /// <param name="fault">The fault.</param>
        /// <param name="principal">The principal.</param>
        public virtual void Fail(TaskEntity task, Fault fault, IPrincipal principal)
        {
            if (task.Status != TaskStatus.InProgress)
                throw new TaskInvalidStateException();
            if (!principal.IsInRole(HumanRoles.ActualOwner))
                throw new TaskAccessException();
            task.Status = TaskStatus.Failed;
        }

        /// <summary>
        /// Skips the task. This action will be successfull only if the task is Skippable.
        /// This action can be performed by an Actual Owner, a Business Administrator, or Task initiator
        /// <exception cref="TaskAccessException"/>
        /// 	<exception cref="TaskInvalidStateException"/>
        /// </summary>
        /// <param name="task">The task.</param>
        /// <param name="principal">The principal.</param>
        public virtual void Skip(TaskEntity task, IPrincipal principal)
        {
            if (!task.IsSkippable)
                throw new TaskInvalidStateException();
            if (!principal.IsInAnyRole(HumanRoles.ActualOwner, HumanRoles.BusinessAdministrator, HumanRoles.Initiator))
                throw new TaskAccessException();
            task.Status = TaskStatus.Completed;
            task.Completed = DateTime.UtcNow;
        }

        /// <summary>
        /// Forwards this instance.
        /// This action can be performed by an Actual Owner, a Business Administrator,
        /// or a Potential Owner (if the task is in 'Ready' state
        /// <exception cref="TaskAccessException"/>
        /// 	<exception cref="TaskInvalidStateException"/>
        /// </summary>
        /// <param name="task">The task entity</param>
        /// <param name="target">The target identity ( a person or a group) .</param>
        /// <param name="principal">The principal.</param>
        public virtual void Forward(TaskEntity task, IIdentity target, IPrincipal principal)
        {
            switch (task.Status)
            {
                case TaskStatus.Ready:
                    if (principal.IsInAnyRole(HumanRoles.ActualOwner, HumanRoles.PotentialOwner,
                        HumanRoles.BusinessAdministrator))
                    {
                        ExecuteForward(task, target);
                    }
                    else
                    {
                        throw new TaskAccessException();
                    }
                    break;
                case TaskStatus.Reserved:
                case TaskStatus.InProgress:
                    if (principal.IsInAnyRole(HumanRoles.ActualOwner, HumanRoles.BusinessAdministrator))
                    {
                        ExecuteForward(task, target);
                    }
                    else
                    {
                        throw new TaskAccessException();
                    }
                    break;
                default:
                    throw new TaskInvalidStateException();
            }
        }

        /// <summary>
        /// Delegates the task.
        /// Assign the task to a target user and set the task status to 'Reserved'.
        /// If the recipient was not a potential owner then this
        /// person will be added to the list of potential owners.
        /// </summary>
        /// <param name="task">The task entity</param>
        /// <param name="target">The target user.</param>
        /// <param name="priority">The priority.</param>
        /// <param name="principal">The principal.</param>
        public virtual void Delegate(TaskEntity task, IIdentity target, Priority priority, IPrincipal principal)
        {
            if (task.Status != TaskStatus.InProgress)
                throw new TaskInvalidStateException();
            if (!principal.IsInAnyRole(HumanRoles.ActualOwner, HumanRoles.BusinessAdministrator))
                throw new TaskAccessException();
            task.Status = TaskStatus.Reserved;
            if (target.IsGroupIdentity())
                throw new ArgumentException("Cannot delegate a task to a group!");
            task.ActualOwner = target;
            task.Priority = priority;
        }

        /// <summary>
        /// Activates the task. The task state changes from  'Created' to 'Ready' or 'Reserved'
        /// The value of the status depends on the number of Potential Owners.
        /// If there is only one potential owner, then the status will be set to 'Reserved' and the Potential Owner
        /// becomes an Actual Owner.
        /// This action can be performed by a Business Administrator
        /// </summary>
        /// <param name="task">The task.</param>
        /// <param name="potentialOwners">The potential owners.</param>
        /// <param name="principal">The principal.</param>
        /// <exception cref="TaskAccessException"/>
        /// <exception cref="TaskInvalidStateException"/>
        public virtual void Activate(TaskEntity task, IEnumerable<IIdentity> potentialOwners, IPrincipal principal)
        {
            if (task.Status != TaskStatus.Created)
                throw new TaskInvalidStateException();
            if (!principal.IsInRole(HumanRoles.BusinessAdministrator))
                throw new TaskAccessException();

            if (potentialOwners.Count() == 1 && !potentialOwners.ElementAt(0).IsGroupIdentity())
            {
                task.ActualOwner = potentialOwners.ElementAt(0);
                task.Status = TaskStatus.Reserved;
            }
            else
            {
                task.Status = TaskStatus.Ready;
            }
        }

        /// <summary>
        /// Nominate an Identity to process the task. When a  person gets
        /// nominated then the task status changes to 'Reserved' and the person becomes Actual Owner. if
        /// a group gets nominated, then  the task status will be changed to 'Rady'. Each nominated
        /// entity should be added to the collection of Potential Owners.
        /// This action can be performed by a Business Administrator when the task is in 'Created' state
        /// </summary>
        /// <param name="task">The task.</param>
        /// <param name="target">Actual Owner or Potential Owners</param>
        /// <param name="principal">The principal.</param>
        /// <exception cref="TaskAccessException"/>
        /// <exception cref="TaskInvalidStateException"/>
        public virtual void Nominate(TaskEntity task, IIdentity target, IPrincipal principal)
        {
            if (task.Status != TaskStatus.Created)
                throw new TaskInvalidStateException();
            if (!principal.IsInRole(HumanRoles.BusinessAdministrator))
                throw new TaskAccessException();
            if (target.IsGroupIdentity())
            {
                task.Status = TaskStatus.Ready;
            }
            else
            {
                task.Status = TaskStatus.Reserved;
                task.ActualOwner = target;
            }
        }

        /// <summary>
        /// Executes the forwarding of the task.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <param name="target">The target.</param>
        protected virtual void ExecuteForward(TaskEntity task, IIdentity target)
        {
            if (target.IsGroupIdentity())
                throw new ArgumentException("Cannot forward a task to a group!");
            task.Status = TaskStatus.Ready;
            task.ActualOwner = target;
        }


    }
}
