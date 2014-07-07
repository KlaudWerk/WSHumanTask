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
using Klaudwerk.PropertySet;

namespace HumanTask
{
    [Serializable]
    public class TaskAccessException:Exception
    {
        
    }
    [Serializable]
    public class TaskInvalidStateException : Exception
    {

    }

    public class Fault
    {
        
    }

    public class Outcome
    {
        
    }

    public interface ITaskSearchCriteria
    {
        
    }


    /// <summary>
    /// ITask facade interface
    /// </summary>
    public interface ITask
    {
        /// <summary>
        /// Gets the id.
        /// </summary>
        TaskId Id { get; }
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        TaskStatus Status { get; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        string Name { get; set; }
        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        /// <value>The subject.</value>
        string Subject { get; set; }
        /// <summary>
        /// Gets or sets the priority.
        /// Can be executed by an Actual Owner, or a Business Administrator
        /// </summary>
        /// <exception cref="TaskAccessException"></exception>
        /// <value>
        /// The priority.
        /// </value>
        Priority Priority { get; set; }
        /// <summary>
        /// Gets a value indicating whether this task can be skipped
        /// Can be executed by a Business Administrator
        /// </summary>
        /// <exception cref="TaskAccessException"></exception>
        /// <value>
        /// 	<c>true</c> if this instance is skippable; otherwise, <c>false</c>.
        /// </value>
        bool IsSkippable { get; set; }
        /// <summary>
        /// Gets the task created date.
        /// </summary>
        DateTime Created { get;  }
        /// <summary>
        /// Gets the time when the task instance was started.
        /// </summary>
        DateTime? Started { get; }
        /// <summary>
        /// Gets the time when the task instance was completed completed.
        /// </summary>
        DateTime? Completed { get; }
        /// <summary>
        /// Gets the task initiator.
        /// </summary>
        IIdentity Initiator { get; }
        /// <summary>
        /// Gets or sets the task's actual owner.
        /// </summary>
        /// <value>
        /// The actual owner.
        /// </value>
        IIdentity ActualOwner { get; }
        /// <summary>
        /// Gets the potential owners.
        /// </summary>
        IEnumerable<IIdentity> PotentialOwners { get;  }
        /// <summary>
        /// Gets the excluded owners.
        /// </summary>
        IEnumerable<IIdentity> ExcludedOwners { get; }
        /// <summary>
        /// Gets the stakeholders.
        /// </summary>
        IEnumerable<IIdentity> Stakeholders { get; }
        /// <summary>
        /// Gets the business administrators.
        /// </summary>
        IEnumerable<IIdentity> BusinessAdministrators { get; }
        /// <summary>
        /// Gets the recepients.
        /// </summary>
        IEnumerable<IIdentity> Recepients { get; }
        /// <summary>
        /// Gets the potential delegatees.
        /// </summary>
        IEnumerable<IIdentity> PotentialDelegatees { get; }

        /// <summary>
        /// Gets the history of the task.
        /// </summary>
        IEnumerable<TaskHistoryEvent> History { get; }
        /// <summary>
        /// Gets the comments.
        /// </summary>
        ICollection<TaskComment> Comments { get; }

        /// <summary>
        /// Gets the ad-hoc attachments collection.
        /// </summary>
        IAttachmentCollection Attachments { get; }
        /// <summary>
        /// Gets the procesing data.
        /// </summary>
        IPropertySetCollection ProcesingData { get; }
        /// <summary>
        /// Gets the collection of associated deadlines
        /// </summary>
        /// <value>The deadlines.</value>
        ICollection<Deadline> Deadlines { get; }

        /// <summary>
        /// Gets the notifications.
        /// </summary>
        /// <value>The notifications.</value>
        ICollection<INotificationDefinition> Notifications { get; }

        /// <summary>
        /// The collection of notifications received by this task
        /// </summary>
        /// <value>The received notification.</value>
        IEnumerable<INotification> ReceivedNotification { get; }

        /// <summary>
        /// Gets the parent task.
        /// </summary>
        /// <value>The parent.</value>
        ITask Parent { get; set; }
        /// <summary>
        /// Gets the subtasks.
        /// </summary>
        /// <value>The subtasks.</value>
        IEnumerable<ITask> Subtasks { get; }
        /// <summary>
        /// Gets the task outcome.
        /// </summary>
        /// <value>The task outcome.</value>
        string TaskOutcome { get; set; }

        #region Task Life cycle Operations

        /// <summary>
        /// Claim responsibility for a task. The task to status will be set to 'Reserved'
        /// The claimant must be in the list of Potential Owners or Business Administrators.
        /// The Actual owner will be set to Claimant
        /// </summary>
        /// <exception cref="TaskAccessException"/>
        /// <exception cref="TaskInvalidStateException"/>
        /// <returns></returns>
        void Claim();
        /// <summary>
        /// Starts the task, The task status will be set to 'InProgress'.
        /// This action can be taken by an Actual Owner or a Potential Owner, if the task is in 'Ready' state.
        /// </summary>
        /// <exception cref="TaskAccessException"/>
        /// <exception cref="TaskInvalidStateException"/>
        /// <returns></returns>
        void Start();
        /// <summary>
        /// Stops the task. Cancel/stop the processing of the task. The task returns to the 'Reserved' state.
        /// This action can be performed by an Actual Owner or a Business Administrator
        /// <exception cref="TaskAccessException"/>
        /// <exception cref="TaskInvalidStateException"/>
        /// </summary>
        void Stop();
        /// <summary>
        /// Releases the task, The task status will be set back 'Ready'.
        /// This action can be performed by an Actual Owner or a Business Administrator
        /// <exception cref="TaskAccessException"/>
        /// <exception cref="TaskInvalidStateException"/>
        /// </summary>
        void Release();
        /// <summary>
        /// <summary>
        /// Suspends the until. Thetask status will be set as 'Suspended'
        /// This action can be performed by  Potential Owners(only if the task in 'Ready' state), 
        /// an Actual Owner, or a Business Administrator
        /// </summary>
        /// <exception cref="TaskAccessException"/>
        /// <exception cref="TaskInvalidStateException"/>
        /// </summary>
        void Suspend();
        /// <summary>
        /// Suspend the task until to a given date.
		/// This action can be performed by  Potential Owners(only if the task in 'Ready' state), 
        /// an Actual Owner, or a Business Administrator
        /// </summary>
        /// <exception cref="TaskAccessException"/>
        /// <exception cref="TaskInvalidStateException"/>
        /// <param name="nextTime">The next time.</param>
        void SuspendUntil(DateTime nextTime);
        /// <summary>
        /// Suspend the task for a given period of time.
        /// This action can be performed by Potential Owners(only if the task in 'Ready' state), 
        /// an Actual Owner, or a Business Administrator
        /// </summary>
        /// <exception cref="TaskAccessException"/>
        /// <exception cref="TaskInvalidStateException"/>
        /// <param name="timePeriod">The time period.</param>
        void SuspendUntil(TimeSpan timePeriod);
        /// <summary>
        /// Resumes the suspended task.
        /// The task status will be set to the value that the task had before 'Suspend' action was called.
        /// This action can be performed by  Potential Owners(only if the task in 'Ready' state), 
        /// an Actual Owner, or a Business Administrator
        /// </summary>
        /// <exception cref="TaskAccessException"/>
        /// <exception cref="TaskInvalidStateException"/>
        void Resume();
        /// <summary>
        /// Completes the task successfully finished.
        /// This action can only be performed by an Actual Owner.
        /// </summary>
        /// <exception cref="TaskAccessException"/>
        /// <exception cref="TaskInvalidStateException"/>
        /// <returns>Final outcome of the task</returns>
        void Complete();
        /// <summary>
        /// Fails the task.
        /// Completes the execution of the task by raising a fault.
        /// This action can only be performed by an Actual Owner.
        /// <exception cref="TaskAccessException"/>
        /// <exception cref="TaskInvalidStateException"/>
        /// </summary>
        /// <param name="fault">The fault.</param>
        void Fail(Fault fault);
        /// <summary>
        /// Skips the task. This action will be successfull only if the task is Skippable. 
        /// This action can be performed by an Actual Owner, a Business Administrator, or Task initiator
        /// <exception cref="TaskAccessException"/>
        /// <exception cref="TaskInvalidStateException"/>
        /// </summary>
        void Skip();
        /// <summary>
        /// Forwards this instance.
        /// This action can be performed by an Actual Owner, a Business Administrator, 
        /// or a Potential Owner (if the task is in 'Ready' state
        /// <exception cref="TaskAccessException"/>
        /// <exception cref="TaskInvalidStateException"/>
        /// </summary>
        /// <param name="target">The target identity ( a person or a group) .</param>
        void Forward(IIdentity target);
        /// <summary>
        /// Delegates the task.
        /// Assign the task to a target user and set the task status to 'Reserved'. 
        /// If the recipient was not a potential owner then this
		///	person will be added to the list of potential owners. 
		/// </summary> 
        /// <param name="target">The target user.</param>
        /// <param name="priority">The priority.</param>
        void Delegate(IIdentity target, Priority priority);

        #endregion
        #region Task Administrator operations
        /// <summary>
        /// Activates the task. The task state changes from  'Created' to 'Ready' or 'Reserved'
		///	The value of the status depends on the number of Potential Owners. 
		///	If there is only one potential owner, then the status will be set to 'Reserved' and the Potential Owner 
		///	becomes an Actual Owner.		
        /// This action can be performed by a Business Administrator
        /// </summary>
        ///<exception cref="TaskAccessException"/>
        /// <exception cref="TaskInvalidStateException"/>
        void Activate();
        /// <summary>
        /// Nominate an Identity to process the task. When a  person gets 
		///	nominated then the task status changes to 'Reserved' and the person becomes Actual Owner. if 
		///	a group gets nominated, then  the task status will be changed to 'Rady'. Each nominated 
		///	entity should be added to the collection of Potential Owners. 
        /// This action can be performed by a Business Administrator when the task is in 'Created' state
        /// </summary>
        ///<exception cref="TaskAccessException"/>
        ///<exception cref="TaskInvalidStateException"/>
        ///<param name="target">Actual Owner or Potential Owners</param>
        void Nominate(IIdentity target);

        #endregion

    }
}