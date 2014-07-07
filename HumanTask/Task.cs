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
using System.Reflection;
using System.Security.Principal;
using System.Threading;
using HumanTask.Security;
using Klaudwerk.PropertySet;
using KlaudWerk.Security;
using log4net;

namespace HumanTask
{
    /// <summary>
    /// The Task Facade implementation
    /// </summary>
    public class Task : IEquatable<Task>
    {
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly TaskLifecycle _lifecycle=new TaskLifecycle();
        private readonly TaskEntity _entity;

        [ThreadStatic]
        private IPrincipal _taskPrincipal;

        /// <summary>
        /// Gets the entity.
        /// </summary>
        protected TaskEntity Entity
        {
            get { return _entity; }
        }

        /// <summary>
        /// Gets the id.
        /// </summary>
        public TaskId Id
        {
            get { return _entity.TaskId; }
        }

        /// <summary>
        /// Gets or sets the logging service.
        /// </summary>
        /// <value>The logging service.</value>
        public virtual ILoggingService LoggingService
        {
            get; set;
        }

        /// <summary>
        /// Gets the current principal.
        /// </summary>
        /// <value>The current principal.</value>
        private IPrincipal TaskPrincipal
        {
            get
            {
                if(_taskPrincipal==null)
                {
                    _taskPrincipal = new TaskPrincipal(Thread.CurrentPrincipal, _entity);
                }
                else
                {
                    IPrincipal principal = Thread.CurrentPrincipal;
                    if (principal.Identity != _taskPrincipal.Identity)
                        _taskPrincipal = new TaskPrincipal(principal, _entity);

                }
                return _taskPrincipal;
            }
        }
        /// <summary>
        /// Gets or sets the status.
        /// Updating the status generates a Task History entry
        /// </summary>
        /// <value>The status.</value>
        public TaskStatus Status
        {
            get
            {
                return _entity.Status;
            }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name
        {
            get { return _entity.Name; }
            set
            {
                OnSetName(value); 
            }
        }

        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        /// <value>The subject.</value>
        public virtual string Subject
        {
            get
            {
                return _entity.Subject;
            }
            set
            {
                OnSetSubject(value);
            }
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
            get
            {
                return _entity.Priority;
            } 
            set
            {
                OnSetPriority(value);
            }
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
            get
            {
                return _entity.IsSkippable;
            }
            set
            {
                OnSetSkippable(value); 
            }
        }

        /// <summary>
        /// Gets the task created date.
        /// </summary>
        public virtual DateTime Created { get { return _entity.Created; } }

        /// <summary>
        /// Gets the time when the task instance was started.
        /// </summary>
        public virtual DateTime? Started { get { return _entity.Started; } }

        /// <summary>
        /// Gets the time when the task instance was completed completed.
        /// </summary>
        public virtual DateTime? Completed { get { return _entity.Completed; } }

        /// <summary>
        /// Gets the task initiator.
        /// </summary>
        public virtual IIdentity Initiator
        {
            get
            {
                return _entity.Initiator;
            }
        }

        /// <summary>
        /// Gets or sets the task's actual owner.
        /// </summary>
        /// <value>
        /// The actual owner.
        /// </value>
        public virtual IIdentity ActualOwner
        {
            get
            {
                return _entity.ActualOwner;
            }
        }

        /// <summary>
        /// Gets the potential owners.
        /// </summary>
        public virtual ICollection<IIdentity> PotentialOwners 
        { 
            get { return _entity.PotentialOwners; }
        }

        /// <summary>
        /// Gets the excluded owners.
        /// </summary>
        public virtual ICollection<IIdentity> ExcludedOwners
        {
            get { return _entity.ExcludedOwners; }
        }

        /// <summary>
        /// Gets the stakeholders.
        /// </summary>
        public virtual ICollection<IIdentity> Stakeholders
        {
            get { return _entity.Stakeholders; }
        }

        /// <summary>
        /// Gets the business administrators.
        /// </summary>
        public virtual ICollection<IIdentity> BusinessAdministrators
        {
            get { return _entity.BusinessAdministrators; }
        }

        /// <summary>
        /// Gets the recepients.
        /// </summary>
        public virtual ICollection<IIdentity> Recepients
        {
            get { return _entity.Recepients; }
        }

        /// <summary>
        /// Gets the potential delegatees.
        /// </summary>
        public virtual ICollection<IIdentity> PotentialDelegatees
        {
            get { return _entity.PotentialDelegatees; }
        }

        /// <summary>
        /// Gets the history of the task.
        /// </summary>
        public IEnumerable<TaskHistoryEvent> History
        {
            get { return LoggingService.History; }
        }

        /// <summary>
        /// Gets the comments.
        /// </summary>
        public virtual IEnumerable<TaskComment> Comments
        {
            get { return _entity.Comments; }  
        }

        /// <summary>
        /// Gets the ad-hoc attachments collection.
        /// </summary>
        public virtual IAttachmentCollection Attachments { get; set; }

        /// <summary>
        /// Gets the procesing data.
        /// </summary>
        public virtual IPropertySetCollection ProcesingData { get; set; }


        /// <summary>
        /// Gets the collection of associated deadlines
        /// </summary>
        /// <value>The deadlines.</value>
        public virtual ICollection<Deadline> Deadlines { get; set; }

        /// <summary>
        /// Gets the notifications.
        /// </summary>
        /// <value>The notifications.</value>
        public virtual ICollection<INotificationDefinition> Notifications { get;  set; }

        /// <summary>
        /// The collection of notifications received by this task
        /// </summary>
        /// <value>The received notification.</value>
        public virtual IEnumerable<INotification> ReceivedNotification { get; set; }

        /// <summary>
        /// Gets the parent task.
        /// </summary>
        /// <value>The parent.</value>
        public Task Parent
        {
            get { return _entity.Parent == null ? null : new Task(_entity.Parent); } 
            set { _entity.Parent = value._entity; }
        }

        /// <summary>
        /// Gets the subtasks.
        /// </summary>
        /// <value>The subtasks.</value>
        public IEnumerable<Task> Subtasks
        {
            get { return _entity.Subtasks.Select(e => new Task(e)); }
        }

        /// <summary>
        /// Gets the task outcome.
        /// </summary>
        /// <value>The task outcome.</value>
        public string TaskOutcome
        {
            get
            {
                return _entity.TaskOutcome;
            }
            set
            {
                OnSetOutcome(value);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Task"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="status">The status.</param>
        /// <param name="name">The name.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="priority">The priority.</param>
        /// <param name="isSkippable">if set to <c>true</c> [is skippable].</param>
        /// <param name="created">The created.</param>
        /// <param name="initiator">The initiator.</param>
        /// <param name="started">The started.</param>
        /// <param name="completed">The completed.</param>
        /// <param name="actualOwner">The actual owner.</param>
        public Task(TaskId id,
            TaskStatus status,
            string name,
            string subject,
            Priority priority,
            bool isSkippable,
            DateTime created,
            IIdentity initiator,
            DateTime? started,
            DateTime? completed,
            IIdentity actualOwner)
            : this(new TaskEntity
                {
                    TaskId = id,
                    Status = status,
                    Name = name,
                    Subject = subject,
                    Priority = priority,
                    Created = created,
                    Started = started,
                    Completed = completed,
                    IsSkippable = isSkippable,
                    Initiator = initiator,
                    ActualOwner = actualOwner
                })
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Task"/> class.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public Task(TaskEntity entity)
        {
            _entity = entity;
        }

        public virtual void AddComment(string taskComment)
        {
            TaskComment comment = new TaskComment
                                      {
                                          Comment = taskComment,
                                          TimeStamp = DateTime.UtcNow,
                                          UserId = TaskPrincipal.Identity.GetMappedId()
                                      };
            Entity.Comments.Add(comment);
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
            LogEvent("Claim", OnClaimTask);
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
            LogEvent("Start", OnStartTask);
        }
        /// <summary>
        /// Stops the task. Cancel/stop the processing of the task. The task returns to the 'Reserved' state.
        /// This action can be performed by an Actual Owner or a Business Administrator
        /// <exception cref="TaskAccessException"/>
        /// <exception cref="TaskInvalidStateException"/>
        /// </summary>
        public void Stop()
        {
            LogEvent("Stop", OnStopTask);
        }
        /// <summary>
        /// Releases the task, The task status will be set back 'Ready'.
        /// This action can be performed by an Actual Owner or a Business Administrator
        /// <exception cref="TaskAccessException"/>
        /// <exception cref="TaskInvalidStateException"/>
        /// </summary>
        public void Release()
        {
            LogEvent("Release", OnRelease);
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
            LogEvent("Suspend",OnSuspend);
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
            LogEvent("SuspendUntil", ()=>OnSuspendUntil(nextTime));
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
            LogEvent("SuspendUntil", () => OnSuspendUntil(timePeriod));
        }

        /// <summary>
        /// Resumes the suspended task.
        /// The task status will be set to the value that the task had before 'Suspend' action was called.
        /// This action can be performed by  Potential Owners(only if the task in 'Ready' state), 
        /// an Actual Owner, or a Business Administrator
        /// </summary>
        /// <exception cref="TaskAccessException"/>
        /// <exception cref="TaskInvalidStateException"/>
        public virtual void Resume()
        {
            LogEvent("Resume",OnResume);
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
            LogEvent("Complete",OnComplete);
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
            LogEvent("Fail",()=>OnFail(fault));
        }
        /// <summary>
        /// Skips the task. This action will be successfull only if the task is Skippable. 
        /// This action can be performed by an Actual Owner, a Business Administrator, or Task initiator
        /// <exception cref="TaskAccessException"/>
        /// <exception cref="TaskInvalidStateException"/>
        /// </summary>
        public void Skip()
        {
            LogEvent("Skip",OnSkip);
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
            LogEvent("Forward",()=>OnForward(target));
        }
        /// <summary>
        /// Delegates the task.
        /// Assign the task to a target user and set the task status to 'Reserved'. 
        /// If the recipient was not a potential owner then this
        ///	person will be added to the list of potential owners. 
        /// This action can be performed by an Actual Owner or a Business Administrator
        /// </summary> 
        /// <param name="target">The target user.</param>
        /// <param name="priority">The priority.</param>
        public  void Delegate(IIdentity target, Priority priority)
        {
            LogEvent("Delegate",()=>OnDelegate(target,priority));
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
            LogEvent("Activate",OnActivate);
        }
        /// <summary>
        /// Nominate an Identity to process the task. When a  person gets 
        ///	nominated then the task status changes to 'Reserved' and the person becomes Actual Owner. if 
        ///	a group gets nominated, then  the task status will be changed to 'Ready'. Each nominated 
        ///	entity should be added to the collection of Potential Owners. 
        /// This action can be performed by a Business Administrator when the task is in 'Created' state
        /// </summary>
        ///<exception cref="TaskAccessException"/>
        ///<exception cref="TaskInvalidStateException"/>
        ///<param name="target">Actual Owner or Potential Owners</param>
        public void Nominate(IIdentity target)
        {
            LogEvent("Nominate",()=>OnNominate(target));
        }

        #region Protected Methods

        /// <summary>
        /// Adds the potential owner.
        /// </summary>
        /// <param name="target">The target.</param>
        protected virtual void AddPotentialOwner(IIdentity target)
        {
            PotentialOwners.Add(target);
        }

        /// <summary>
        /// Called when the task priority is getting set
        /// </summary>
        /// <param name="value">The value.</param>
        protected virtual void OnSetPriority(Priority value)
        {
            LogEvent("Priority", ()=>_entity.Priority = value );
        }

        /// <summary>
        /// Called when setting Skippable property on a task
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        protected virtual void OnSetSkippable(bool value)
        {
             _entity.IsSkippable = value;
        }
        /// <summary>
        /// Set the task name
        /// </summary>
        /// <param name="value">The value.</param>
        protected virtual void OnSetName(string value)
        {
             _entity.Name = value;
        }

        /// <summary>
        /// Set the subject.
        /// </summary>
        /// <param name="value">The value.</param>
        protected virtual void OnSetSubject(string value)
        {
             _entity.Subject = value;
        }

        /// <summary>
        /// Set the task outcome
        /// </summary>
        /// <param name="value">The value.</param>
        protected virtual void OnSetOutcome(string value)
        {
             _entity.TaskOutcome = value;
        }

        /// <summary>
        /// Performs the actual Claim(..) operation
        /// Claim responsibility for a task. The task to status will be set to 'Reserved'
        /// The claimant must be in the list of Potential Owners or Business Administrators.
        /// The Actual owner will be set to Claimant
        /// </summary>
        protected virtual void OnClaimTask()
        {
            _lifecycle.Claim(_entity,TaskPrincipal);
        }


        /// <summary>
        /// Performs the actual Start(..) operation 
        /// Starts the task, The task status will be set to 'InProgress'.
        /// This action can be taken by an Actual Owner or a Potential Owner, if the task is in 'Ready' state. 
        /// </summary>
        protected virtual void OnStartTask()
        {
            _lifecycle.Start(_entity,TaskPrincipal);
        }

        /// <summary>
        /// Stops the task. Cancel/stop the processing of the task. The task returns to the 'Reserved' state.
        /// This action can be performed by an Actual Owner or a Business Administrator
        /// <exception cref="TaskAccessException"/>
        /// <exception cref="TaskInvalidStateException"/>
        /// </summary>
        protected virtual void OnStopTask()
        {
            _lifecycle.Stop(_entity,TaskPrincipal);
        }

        /// <summary>
        /// Releases the task, The task status will be set back 'Ready'.
        /// This action can be performed by an Actual Owner or a Business Administrator
        /// <exception cref="TaskAccessException"/>
        /// <exception cref="TaskInvalidStateException"/>
        /// </summary>
        protected virtual void OnRelease()
        {
            _lifecycle.Release(_entity,TaskPrincipal);
        }

        /// <summary>
        /// Resumes the suspended task.
        /// The task status will be set to the value that the task had before 'Suspend' action was called.
        /// This action can be performed by  Potential Owners(only if the task in 'Ready' state), 
        /// an Actual Owner, or a Business Administrator
        /// </summary>
        /// <exception cref="TaskAccessException"/>
        /// <exception cref="TaskInvalidStateException"/>
        protected virtual void OnResume()
        {
            _lifecycle.Resume(_entity,TaskPrincipal);
        }

        /// <summary>
        /// Completes the task successfully finished.
        /// This action can only be performed by an Actual Owner.
        /// </summary>
        /// <exception cref="TaskAccessException"/>
        /// <exception cref="TaskInvalidStateException"/>
        /// <returns>Final outcome of the task</returns>
        protected virtual void OnComplete()
        {
            if (Subtasks.Any(subtask => subtask.Status != TaskStatus.Completed))
            {
                throw new TaskInvalidStateException();
            }
            _lifecycle.Complete(_entity,TaskPrincipal);
        }

        /// <summary>
        /// Fails the task.
        /// Completes the execution of the task by raising a fault.
        /// This action can only be performed by an Actual Owner.
        /// <exception cref="TaskAccessException"/>
        /// <exception cref="TaskInvalidStateException"/>
        /// </summary>
        /// <param name="fault">The fault.</param>
        protected virtual void OnFail(Fault fault)
        {
            _lifecycle.Fail(_entity, fault, TaskPrincipal);
        }

        /// <summary>
        /// Implementation of Skips the task. This action will be successfull only if the task is Skippable. 
        /// This action can be performed by an Actual Owner, a Business Administrator, or Task initiator
        /// <exception cref="TaskAccessException"/>
        /// <exception cref="TaskInvalidStateException"/>
        /// </summary>
        protected virtual void OnSkip()
        {
            _lifecycle.Skip(_entity,TaskPrincipal);
        }

        /// <summary>
        /// Implementation of the Instance Forwarding
        /// This action can be performed by an Actual Owner, a Business Administrator, 
        /// or a Potential Owner (if the task is in 'Ready' state
        /// <exception cref="TaskAccessException"/>
        /// <exception cref="TaskInvalidStateException"/>
        /// </summary>
        /// <param name="target">The target identity ( a person or a group) .</param>
        protected virtual void OnForward(IIdentity target)
        {
            _lifecycle.Forward(_entity,target,TaskPrincipal);
            AddPotentialOwner(target);
        }
        /// <summary>
        /// Real implementation of Delegates the task.
        /// Assign the task to a target user and set the task status to 'Reserved'. 
        /// If the recipient was not a potential owner then this
        ///	person will be added to the list of potential owners. 
        /// This action can be performed by an Actual Owner or a Business Administrator
        /// </summary> 
        /// <param name="target">The target user.</param>
        /// <param name="priority">The priority.</param>
        protected virtual void OnDelegate(IIdentity target, Priority priority)
        {

            _lifecycle.Delegate(_entity, target, priority, TaskPrincipal);
            AddPotentialOwner(target);

        }

        /// <summary>
        /// Real implementation of Activate state transition
        /// Activates the task. The task state changes from  'Created' to 'Ready' or 'Reserved'
        ///	The value of the status depends on the number of Potential Owners. 
        ///	If there is only one potential owner, then the status will be set to 'Reserved' and the Potential Owner 
        ///	becomes an Actual Owner.		
        /// This action can be performed by a Business Administrator
        /// </summary>
        ///<exception cref="TaskAccessException"/>
        /// <exception cref="TaskInvalidStateException"/>
        protected virtual void OnActivate()
        {
           _lifecycle.Activate(_entity, PotentialOwners, TaskPrincipal);
        }

        /// <summary>
        /// Real implementation of Nominate state transition
        /// Nominate an Identity to process the task. When a  person gets 
        ///	nominated then the task status changes to 'Reserved' and the person becomes Actual Owner. if 
        ///	a group gets nominated, then  the task status will be changed to 'Ready'. Each nominated 
        ///	entity should be added to the collection of Potential Owners. 
        /// This action can be performed by a Business Administrator when the task is in 'Created' state
        /// </summary>
        ///<exception cref="TaskAccessException"/>
        ///<exception cref="TaskInvalidStateException"/>
        ///<param name="target">Actual Owner or Potential Owners</param>
        protected virtual void OnNominate(IIdentity target)
        {
            _lifecycle.Nominate(_entity, target, TaskPrincipal);
            AddPotentialOwner(target);
        }
        ///	<summary>
        /// Suspends the task. The task status will be set as 'Suspended'
        /// This action can be performed by  Potential Owners(only if the task in 'Ready' state),
        /// an Actual Owner, or a Business Administrator
        /// </summary>
        /// 	<exception cref="TaskAccessException"/>
        /// 	<exception cref="TaskInvalidStateException"/>
        protected virtual void OnSuspend()
        {
            _lifecycle.Suspend(_entity,TaskPrincipal);
        }

        /// <summary>
        /// Suspend the task until to a given date.
        /// This action can be performed by  Potential Owners(only if the task in 'Ready' state),
        /// an Actual Owner, or a Business Administrator
        /// </summary>
        /// <param name="nextTime">The next time.</param>
        /// <exception cref="TaskAccessException"/>
        /// <exception cref="TaskInvalidStateException"/>
        protected virtual void OnSuspendUntil(DateTime nextTime)
        {
            _lifecycle.SuspendUntil(_entity,nextTime,TaskPrincipal);
        }

        /// <summary>
        /// Suspend the task for a given period of time.
        /// This action can be performed by Potential Owners(only if the task in 'Ready' state),
        /// an Actual Owner, or a Business Administrator
        /// </summary>
        /// <param name="timePeriod">The time period.</param>
        /// <exception cref="TaskAccessException"/>
        /// <exception cref="TaskInvalidStateException"/>
        protected virtual void OnSuspendUntil(TimeSpan timePeriod)
        {
            _lifecycle.SuspendUntil(_entity, timePeriod, TaskPrincipal);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Logs the history event.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="a">A.</param>
        private void LogEvent(string name, Action a)
        {
            _logger.DebugEx(() => name);
            Priority originalPriority = Priority;
            TaskStatus originalStatus = Status;
            IIdentity owner = ActualOwner;
            if (a != null)
                a.Invoke();
            LoggingService.LogHistoryEntry(new TaskHistoryEvent
            {
                Event = name,
                OldStatus = originalStatus,
                NewStatus = Status,
                OldPriority = originalPriority,
                NewPriority = Priority,
                StartOwner = owner.GetMappedId(),
                EndOwner = ActualOwner.GetMappedId(),
                Comment = name,
                UserId = TaskPrincipal.Identity.GetMappedId(),
                TimeStamp = DateTime.UtcNow
            });
        }
        #endregion

        #region IEquitable implementation
        public bool Equals(Task other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._entity, _entity);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(Task)) return false;
            return Equals((Task)obj);
        }

        public override int GetHashCode()
        {
            return (_entity != null ? _entity.GetHashCode() : 0);
        }

        public static bool operator ==(Task left, Task right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Task left, Task right)
        {
            return !Equals(left, right);
        }
        #endregion

        /// <summary>
        /// Accepts the specified visitor.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        public virtual void Accept(ITaskVisitor visitor)
        {
            visitor.Visit(this);
            visitor.Visit(Entity);
        }
        
    }
}
