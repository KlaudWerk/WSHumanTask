using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Principal;
using System.Text;
using Klaudwerk.PropertySet;

namespace HumanTask.Mongo
{
    /// <summary>
    /// Mongo Task Proxy
    /// </summary>
    public class MongoTask:ITask
    {
        /// <summary>
        /// Gets the id.
        /// </summary>
        public TaskId Id
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public TaskStatus Status
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        /// <value>The subject.</value>
        public string Subject
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets or sets the priority.
        /// Can be executed by an Actual Owner, or a Business Administrator
        /// </summary>
        /// <exception cref="TaskAccessException"></exception>
        /// <value>
        /// The priority.
        /// </value>
        public Priority Priority
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets a value indicating whether this task can be skipped
        /// Can be executed by a Business Administrator
        /// </summary>
        /// <exception cref="TaskAccessException"></exception>
        /// <value>
        /// 	<c>true</c> if this instance is skippable; otherwise, <c>false</c>.
        /// </value>
        public bool IsSkippable
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the task created date.
        /// </summary>
        public DateTime Created
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the time when the task instance was started.
        /// </summary>
        public DateTime? Started
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the time when the task instance was completed completed.
        /// </summary>
        public DateTime? Completed
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the task initiator.
        /// </summary>
        public IIdentity Initiator
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets or sets the task's actual owner.
        /// </summary>
        /// <value>
        /// The actual owner.
        /// </value>
        public IIdentity ActualOwner
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the potential owners.
        /// </summary>
        public IEnumerable<IIdentity> PotentialOwners
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the excluded owners.
        /// </summary>
        public IEnumerable<IIdentity> ExcludedOwners
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the stakeholders.
        /// </summary>
        public IEnumerable<IIdentity> Stakeholders
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the business administrators.
        /// </summary>
        public IEnumerable<IIdentity> BusinessAdministrators
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the recepients.
        /// </summary>
        public IEnumerable<IIdentity> Recepients
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the potential delegatees.
        /// </summary>
        public IEnumerable<IIdentity> PotentialDelegatees
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the history of the task.
        /// </summary>
        public IEnumerable<TaskHistoryEvent> History
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the comments.
        /// </summary>
        public ICollection<TaskComment> Comments
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the ad-hoc attachments collection.
        /// </summary>
        public IAttachmentCollection Attachments
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the procesing data.
        /// </summary>
        public IPropertySetCollection ProcesingData
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the collection of associated deadlines
        /// </summary>
        /// <value>The deadlines.</value>
        public ICollection<IDeadline> Deadlines
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the notifications.
        /// </summary>
        /// <value>The notifications.</value>
        public ICollection<INotificationDefinition> Notifications
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// The collection of notifications received by this task
        /// </summary>
        /// <value>The received notification.</value>
        public IEnumerable<INotification> ReceivedNotification
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the parent task.
        /// </summary>
        /// <value>The parent.</value>
        public ITask Parent
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the subtasks.
        /// </summary>
        /// <value>The subtasks.</value>
        public IEnumerable<ITask> Subtasks
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the task outcome.
        /// </summary>
        /// <value>The task outcome.</value>
        public Outcome TaskOutcome
        {
            get { throw new NotImplementedException(); }
        }

        public MongoTask()
        {
        }

        /// <summary>
        /// Claim responsibility for a task. The task to status will be set to 'Reserved'
        /// The claimant must be in the list of Potential Owners or Business Administrators.
        /// The Actual owner will be set to Claimant
        /// </summary>
        /// <exception cref="TaskAccessException"/>
        /// <exception cref="TaskInvalidStateException"/>
        /// <returns></returns>
        public void Claim()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Starts the task, The task status will be set to 'InProgress'.
        /// This action can be taken by an Actual Owner or a Potential Owner, if the task is in 'Ready' state.
        /// </summary>
        /// <exception cref="TaskAccessException"/>
        /// <exception cref="TaskInvalidStateException"/>
        /// <returns></returns>
        public void Start()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Stops the task. Cancel/stop the processing of the task. The task returns to the 'Reserved' state.
        /// This action can be performed by an Actual Owner or a Business Administrator
        /// <exception cref="TaskAccessException"/>
        /// <exception cref="TaskInvalidStateException"/>
        /// </summary>
        public void Stop()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Releases the task, The task status will be set back 'Ready'.
        /// This action can be performed by an Actual Owner or a Business Administrator
        /// <exception cref="TaskAccessException"/>
        /// <exception cref="TaskInvalidStateException"/>
        /// </summary>
        public void Release()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// <summary>
        /// Suspends the until. Thetask status will be set as 'Suspended'
        /// This action can be performed by  Potential Owners(only if the task in 'Ready' state), 
        /// an Actual Owner, or a Business Administrator
        /// </summary>
        /// <exception cref="TaskAccessException"/>
        /// <exception cref="TaskInvalidStateException"/>
        /// </summary>
        public void Suspend()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Suspend the task until to a given date.
        /// This action can be performed by  Potential Owners(only if the task in 'Ready' state), 
        /// an Actual Owner, or a Business Administrator
        /// </summary>
        /// <exception cref="TaskAccessException"/>
        /// <exception cref="TaskInvalidStateException"/>
        /// <param name="nextTime">The next time.</param>
        public void SuspendUntil(DateTime nextTime)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Suspend the task for a given period of time.
        /// This action can be performed by Potential Owners(only if the task in 'Ready' state), 
        /// an Actual Owner, or a Business Administrator
        /// </summary>
        /// <exception cref="TaskAccessException"/>
        /// <exception cref="TaskInvalidStateException"/>
        /// <param name="timePeriod">The time period.</param>
        public void SuspendUntil(TimeSpan timePeriod)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Resumes the suspended task.
        /// The task status will be set to the value that the task had before 'Suspend' action was called.
        /// This action can be performed by  Potential Owners(only if the task in 'Ready' state), 
        /// an Actual Owner, or a Business Administrator
        /// </summary>
        /// <exception cref="TaskAccessException"/>
        /// <exception cref="TaskInvalidStateException"/>
        public void Resume()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Completes the task successfully finished.
        /// This action can only be performed by an Actual Owner.
        /// </summary>
        /// <exception cref="TaskAccessException"/>
        /// <exception cref="TaskInvalidStateException"/>
        /// <returns>Final outcome of the task</returns>
        public void Complete()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Fails the task.
        /// Completes the execution of the task by raising a fault.
        /// This action can only be performed by an Actual Owner.
        /// <exception cref="TaskAccessException"/>
        /// <exception cref="TaskInvalidStateException"/>
        /// </summary>
        /// <param name="fault">The fault.</param>
        public void Fail(Fault fault)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Skips the task. This action will be successfull only if the task is Skippable. 
        /// This action can be performed by an Actual Owner, a Business Administrator, or Task initiator
        /// <exception cref="TaskAccessException"/>
        /// <exception cref="TaskInvalidStateException"/>
        /// </summary>
        public void Skip()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Forwards this instance.
        /// This action can be performed by an Actual Owner, a Business Administrator, 
        /// or a Potential Owner (if the task is in 'Ready' state
        /// <exception cref="TaskAccessException"/>
        /// <exception cref="TaskInvalidStateException"/>
        /// </summary>
        /// <param name="target">The target identity ( a person or a group) .</param>
        public void Forward(IIdentity target)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Delegates the task.
        /// Assign the task to a target user and set the task status to 'Reserved'. 
        /// If the recipient was not a potential owner then this
        ///	person will be added to the list of potential owners. 
        /// </summary> 
        /// <param name="target">The target user.</param>
        /// <param name="priority">The priority.</param>
        public void Delegate(IIdentity target, Priority priority)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Activates the task. The task state changes from  'Created' to 'Ready' or 'Reserved'
        ///	The value of the status depends on the number of Potential Owners. 
        ///	If there is only one potential owner, then the status will be set to 'Reserved' and the Potential Owner 
        ///	becomes an Actual Owner.		
        /// This action can be performed by a Business Administrator
        /// </summary>
        ///<exception cref="TaskAccessException"/>
        /// <exception cref="TaskInvalidStateException"/>
        public void Activate()
        {
            throw new NotImplementedException();
        }

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
        public void Nominate(IIdentity target)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Assign the logical human role
        /// </summary>
        /// <param name="assignment">The assignment.</param>
        public void AssingHumanRole(params LogicalHumanRoleAssignment[] assignment)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes the previosly made assignments
        /// </summary>
        /// <param name="name">The name.</param>
        public void RemoveAssignment(params string[] name)
        {
            throw new NotImplementedException();
        }

        private class MonfoTaskImpl:Task
        {
            public MonfoTaskImpl(TaskId id, 
                TaskStatus status, string name, string subject, Priority priority, 
                bool isSkippable, DateTime created, IIdentity initiator, DateTime? started, DateTime? completed, IIdentity actualOwner) 
                : base(id, status, name, subject, priority, isSkippable, created, initiator, started, completed, actualOwner)
            {
            }

            /// <summary>
            /// Gets the parent task.
            /// </summary>
            /// <value>The parent.</value>
            public override ITask Parent
            {
                get { throw new NotImplementedException(); }
            }

            /// <summary>
            /// Adds the potential owner.
            /// </summary>
            /// <param name="target">The target.</param>
            protected override void AddPotentialOwner(IIdentity target)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Gets the current logged-in principal.
            /// </summary>
            /// <returns></returns>
            protected override IPrincipal GetCurrentPrincipal()
            {
                throw new NotImplementedException();
            }
        }
    }
}
