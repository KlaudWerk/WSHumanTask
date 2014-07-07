using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using KlaudWerk.Common;
using KlaudWerk.Security;
using KlaudWerk.Security.Claims;
using KlaudWerk.Security.Exception;
using Microsoft.Practices.ServiceLocation;
using Moq;
using Moq.Protected;
using NUnit.Framework;

namespace HumanTask.Test
{
    /// <summary>
    /// The state transition tests
    /// </summary>
    [TestFixture]
    public class TaskStateTransitionsTest : TaskMocksSetup
    {
        private WindsorContainer _container;
        private readonly Dictionary<Guid,Account> _mapStorage=new Dictionary<Guid, Account>();
        private IAccountFactory _mockAccountFactory;

        [SetUp]
        public void SetUp()
        {
            _mapStorage.Clear();
            _mockAccountFactory = new MockAccountFactory(_mapStorage);
            WindsorServiceLocator locator=new WindsorServiceLocator(_container=new WindsorContainer());
            _container.Register(Component.For(typeof (IAccountFactory)).UsingFactoryMethod(() => _mockAccountFactory));
            ServiceLocator.SetLocatorProvider(()=>locator);
        }

        /// <summary>
        /// Tests the state of the create.
        /// </summary>
        [Test]
        public void TestCreateState()
        {
            IdentityId initiatorId = new IdentityId();
            IPrincipal initiator = new ClaimsPrincipal(initiatorId.GetIdentity());

            Task task = new Task(new TaskId(), TaskStatus.Created, string.Empty,
                                 string.Empty, Priority.Normal, false,
                                 DateTime.UtcNow, initiator.Identity,
                                 null, null, null);

            Thread.CurrentPrincipal = initiator;
            Assert.IsNotNull(task);
            Assert.AreEqual(TaskStatus.Created, task.Status);
            Assert.IsNull(task.ActualOwner);
            Assert.IsNotNull(task.Initiator);
            Assert.AreEqual(initiator.Identity, task.Initiator);
            Assert.AreEqual(Priority.Normal, task.Priority);
            Assert.AreEqual(TaskStatus.Created, task.Status);
        }

        /// <summary>
        /// Tests the create to ready one potential owner.
        /// </summary>
        [Test]
        public void TestCreateToReadyOnePotentialOwner()
        {
            IdentityId initiatorId = new IdentityId();
            IdentityId potentialOwnerId=new IdentityId();
            IdentityId businessAdminId=new IdentityId();
            IPrincipal initiator = new ClaimsPrincipal(initiatorId.GetIdentity());
            _mapStorage[potentialOwnerId.Id] = new Account(potentialOwnerId, "s-1-2-3-4-5", "auth", AccountType.User);
            _mapStorage[businessAdminId.Id] = new Account(businessAdminId, "s-55-2-3-4-5", "auth", AccountType.User);

            ILoggingService loggingService = SetupLoggerMock(new List<TaskHistoryEvent> { 
                                                                     new TaskHistoryEvent
                                                                         {
                                                                            Event = "Create",
                                                                            OldStatus = TaskStatus.None,
                                                                            NewStatus = TaskStatus.Created,
                                                                            OldPriority = Priority.Normal,
                                                                            NewPriority = Priority.Normal,
                                                                            Comment = "",
                                                                            TimeStamp = DateTime.UtcNow,
                                                                            UserId = initiator.Identity.GetMappedId()
                                                                         }
            });
            Thread.CurrentPrincipal = initiator;
            Task task = new Task(new TaskId(), TaskStatus.Created, string.Empty,
                                 string.Empty, Priority.Normal, false,
                                 DateTime.UtcNow, initiator.Identity,
                                 null, null, null)
                            {
                                LoggingService = loggingService
                            };
            Assert.AreEqual(TaskStatus.Created, task.Status);
            task.PotentialOwners.Add(potentialOwnerId.GetIdentity());
            task.BusinessAdministrators.Add(businessAdminId.GetIdentity());
            // Setup the Current Principal as a Business Administrator
            IPrincipal businessAdmin = new ClaimsPrincipal(businessAdminId.GetIdentity());
            Thread.CurrentPrincipal = businessAdmin;
            task.Activate();
            Assert.AreEqual(TaskStatus.Reserved, task.Status);
            Assert.IsNotNull(task.ActualOwner);
            Assert.AreEqual(task.ActualOwner, potentialOwnerId.GetIdentity());
            Assert.IsNotNull(task.History);
            Assert.AreEqual(2, task.History.Count());
            TaskHistoryEvent history = task.History.ElementAt(0);
            Assert.IsNotNull(history);
            Assert.AreEqual(TaskStatus.None, history.OldStatus);
            Assert.AreEqual(TaskStatus.Created, history.NewStatus);
            Assert.AreEqual(initiator.Identity.GetMappedId(), history.UserId);
            history = task.History.ElementAt(1);
            Assert.IsNotNull(history);
        }

        [Test]
        public void TestCreateToReadyOnePotentialOwnerThroughGroup()
        {
            IdentityId initiatorId = new IdentityId();
            IdentityId potentialOwnerGroupId=new IdentityId();
            IdentityId businessAdminId = new IdentityId();
            IPrincipal initiator = new ClaimsPrincipal(initiatorId.GetIdentity());
            _mapStorage[potentialOwnerGroupId.Id] = new Account(potentialOwnerGroupId, "s-1-2-3-4-5", "auth", AccountType.Group);
            _mapStorage[businessAdminId.Id] = new Account(businessAdminId, "s-55-2-3-4-5", "auth", AccountType.User);

            ILoggingService loggingService = SetupLoggerMock(new List<TaskHistoryEvent> { 
                                                                     new TaskHistoryEvent
                                                                         {
                                                                            Event = "Create",
                                                                            OldStatus = TaskStatus.None,
                                                                            NewStatus = TaskStatus.Created,
                                                                            OldPriority = Priority.Normal,
                                                                            NewPriority = Priority.Normal,
                                                                            Comment = "",
                                                                            TimeStamp = DateTime.UtcNow,
                                                                            UserId = initiator.Identity.GetMappedId()
                                                                         }
            });
            Thread.CurrentPrincipal = initiator;
            Task task = new Task(new TaskId(), TaskStatus.Created, string.Empty,
                                 string.Empty, Priority.Normal, false,
                                 DateTime.UtcNow, initiator.Identity,
                                 null, null, null)
            {
                LoggingService = loggingService
            };
            Assert.AreEqual(TaskStatus.Created, task.Status);
            task.PotentialOwners.Add(potentialOwnerGroupId.GetIdentity());
            task.BusinessAdministrators.Add(businessAdminId.GetIdentity());
            // Setup the Current Principal as a Business Administrator
            IPrincipal businessAdmin = new ClaimsPrincipal(businessAdminId.GetIdentity());
            Thread.CurrentPrincipal = businessAdmin;
            task.Activate();
            Assert.AreEqual(TaskStatus.Ready, task.Status);
            Assert.IsNull(task.ActualOwner);
            Assert.IsNotNull(task.History);
            Assert.AreEqual(2, task.History.Count());
            TaskHistoryEvent history = task.History.ElementAt(0);
            Assert.IsNotNull(history);
            Assert.AreEqual(TaskStatus.None, history.OldStatus);
            Assert.AreEqual(TaskStatus.Created, history.NewStatus);
            Assert.AreEqual(initiator.Identity.GetMappedId(), history.UserId);
            history = task.History.ElementAt(1);
            Assert.IsNotNull(history);
        }

        /// <summary>
        /// Tests the create to ready multiple potential owners.
        /// </summary>
        [Test]
        public void TestCreateToReadyMultiplePotentialOwners()
        {
            IdentityId initiatorId = new IdentityId();
            IdentityId potentialOwnerOne = new IdentityId();
            IdentityId potentialOwnerTwo = new IdentityId();
            IdentityId businessAdminId = new IdentityId();
            IPrincipal initiator = new ClaimsPrincipal(initiatorId.GetIdentity());
            _mapStorage[potentialOwnerOne.Id] = new Account(potentialOwnerOne, "s-1-2-3-4-5", "auth", AccountType.User);
            _mapStorage[potentialOwnerTwo.Id] = new Account(potentialOwnerTwo, "s-1-2-3-4-5", "auth", AccountType.User);
            _mapStorage[businessAdminId.Id] = new Account(businessAdminId, "s-55-2-3-4-5", "auth", AccountType.User);

            ILoggingService loggingService = SetupLoggerMock(new List<TaskHistoryEvent> { 
                                                                     new TaskHistoryEvent{
                                                                         Event = "Create",
                                                                         OldStatus = TaskStatus.None,
                                                                         NewStatus = TaskStatus.Created,
                                                                         OldPriority = Priority.Normal,
                                                                         NewPriority = Priority.Normal,
                                                                         Comment = "",
                                                                         TimeStamp = DateTime.UtcNow,
                                                                         UserId = initiator.Identity.GetMappedId()}
            });
            Thread.CurrentPrincipal = initiator;
            Task task = new Task(new TaskId(), TaskStatus.Created, string.Empty,
                                 string.Empty, Priority.Normal, false,
                                 DateTime.UtcNow, initiator.Identity,
                                 null, null, null)
            {
                LoggingService = loggingService
            };
            Assert.AreEqual(TaskStatus.Created, task.Status);
            task.PotentialOwners.Add(potentialOwnerOne.GetIdentity());
            task.PotentialOwners.Add(potentialOwnerTwo.GetIdentity());
            task.BusinessAdministrators.Add(businessAdminId.GetIdentity());
            Assert.IsNotNull(task);
            Assert.AreEqual(TaskStatus.Created, task.Status);
            IPrincipal businessAdmin = new ClaimsPrincipal(businessAdminId.GetIdentity());
            Thread.CurrentPrincipal = businessAdmin;
            task.Activate();
            Assert.AreEqual(TaskStatus.Ready, task.Status);
            Assert.IsNull(task.ActualOwner);
            Assert.IsNotNull(task.History);
            Assert.AreEqual(2, task.History.Count());
            TaskHistoryEvent history = task.History.ElementAt(0);
            Assert.IsNotNull(history);
            Assert.AreEqual(TaskStatus.None, history.OldStatus);
            Assert.AreEqual(TaskStatus.Created, history.NewStatus);
            Assert.AreEqual(initiator.Identity.GetMappedId(), history.UserId);
            history = task.History.ElementAt(1);
            Assert.IsNotNull(history);

        }

        /// <summary>
        /// Tests the ready to reserved.
        /// </summary>
        [Test]
        public void TestReadyToReserved()
        {
            IdentityId initiatorId = new IdentityId();
            IdentityId potentialOwner = new IdentityId();
            IdentityId businessAdminId = new IdentityId();
            IPrincipal initiator = new ClaimsPrincipal(initiatorId.GetIdentity());
            _mapStorage[potentialOwner.Id] = new Account(potentialOwner, "s-1-2-3-4-5", "auth", AccountType.User);
            _mapStorage[businessAdminId.Id] = new Account(businessAdminId, "s-55-2-3-4-5", "auth", AccountType.User);

            ILoggingService loggingService = SetupLoggerMock(new List<TaskHistoryEvent> { 
                                                                     new TaskHistoryEvent
                                                                         {
                                                                             Event = "Create",
                                                                             OldStatus = TaskStatus.None,
                                                                             NewStatus = TaskStatus.Created,
                                                                             OldPriority = Priority.Normal,
                                                                             NewPriority = Priority.Normal,
                                                                             Comment = "",
                                                                             TimeStamp = DateTime.UtcNow,
                                                                             UserId = initiatorId
                                                                         }
            });

            Thread.CurrentPrincipal = initiator;
            Task task = new Task(new TaskId(), TaskStatus.Created, string.Empty,
                                 string.Empty, Priority.Normal, false,
                                 DateTime.UtcNow, initiator.Identity,
                                 null, null, null)
            {
                LoggingService = loggingService
            };
            task.PotentialOwners.Add(potentialOwner.GetIdentity());
            task.PotentialOwners.Add(new IdentityId().GetIdentity());
            task.BusinessAdministrators.Add(businessAdminId.GetIdentity());
            IPrincipal businessAdmin = new ClaimsPrincipal(businessAdminId.GetIdentity());
            Thread.CurrentPrincipal = businessAdmin;
            task.Activate();
            Assert.AreEqual(TaskStatus.Ready, task.Status);
            Assert.IsNull(task.ActualOwner);
            IPrincipal owner = new ClaimsPrincipal(potentialOwner.GetIdentity());
            Thread.CurrentPrincipal = owner;
            task.Claim();
            Assert.AreEqual(TaskStatus.Reserved, task.Status);
            Assert.IsNotNull(task.ActualOwner);
            Assert.AreEqual(potentialOwner.GetIdentity(), task.ActualOwner);

            Assert.IsNotNull(task.History);
            Assert.AreEqual(3, task.History.Count());
            TaskHistoryEvent history = task.History.ElementAt(0);
            Assert.IsNotNull(history);
            Assert.AreEqual(TaskStatus.None, history.OldStatus);
            Assert.AreEqual(TaskStatus.Created, history.NewStatus);
            Assert.AreEqual(initiatorId, history.UserId);

            history = task.History.ElementAt(1);
            Assert.IsNotNull(history);
            Assert.AreEqual(TaskStatus.Created, history.OldStatus);
            Assert.AreEqual(TaskStatus.Ready, history.NewStatus);
            Assert.AreEqual(businessAdminId, history.UserId);
            history = task.History.ElementAt(2);
            Assert.IsNotNull(history);
            Assert.AreEqual(TaskStatus.Ready, history.OldStatus);
            Assert.AreEqual(TaskStatus.Reserved, history.NewStatus);
            Assert.AreEqual(potentialOwner, history.UserId);
        }

        [Test]
        public void TestReservedToInProgressActualOwner()
        {
            IdentityId actualId = new IdentityId();
            ILoggingService loggingService = SetupLoggerMock(new List<TaskHistoryEvent>());
            Task task = new Task(
                         new TaskId(), TaskStatus.Reserved, string.Empty,
                         string.Empty, Priority.Normal, false,
                         DateTime.UtcNow, new IdentityId().GetIdentity(),
                         DateTime.UtcNow, null, actualId.GetIdentity())
            {
                LoggingService = loggingService
            };

        
            Assert.IsNotNull(task);
            IPrincipal actualOwner = new ClaimsPrincipal(actualId.GetIdentity());
            Thread.CurrentPrincipal = actualOwner;
            task.Start();
            Assert.AreEqual(TaskStatus.InProgress, task.Status);
            Assert.AreEqual(actualId.GetIdentity(), task.ActualOwner);
            Assert.IsNotNull(task.History);
            Assert.AreEqual(1, task.History.Count());
            TaskHistoryEvent history = task.History.ElementAt(0);
            Assert.IsNotNull(history);
            Assert.AreEqual(TaskStatus.Reserved, history.OldStatus);
            Assert.AreEqual(TaskStatus.InProgress, history.NewStatus);
            Assert.AreEqual(actualId, history.UserId);
        }

        [Test]
        public void TestInProgressToCompletedActualOwner()
        {
            IdentityId actualId = new IdentityId();
            ILoggingService loggingService = SetupLoggerMock(new List<TaskHistoryEvent>());
            Task task = new Task(
                         new TaskId(), TaskStatus.InProgress, string.Empty,
                         string.Empty, Priority.Normal, false,
                         DateTime.UtcNow, new IdentityId().GetIdentity(),
                         DateTime.UtcNow, null, actualId.GetIdentity())
            {
                LoggingService = loggingService
            };


            Assert.IsNotNull(task);
            IPrincipal actualOwner = new ClaimsPrincipal(actualId.GetIdentity());
            Thread.CurrentPrincipal = actualOwner;
            task.Complete();
            Assert.AreEqual(TaskStatus.Completed, task.Status);
            Assert.AreEqual(actualId.GetIdentity(), task.ActualOwner);
            Assert.IsNotNull(task.History);
            Assert.AreEqual(1, task.History.Count());
            TaskHistoryEvent history = task.History.ElementAt(0);
            Assert.IsNotNull(history);
            Assert.AreEqual(TaskStatus.InProgress, history.OldStatus);
            Assert.AreEqual(TaskStatus.Completed, history.NewStatus);
            Assert.AreEqual(actualId, history.UserId);

        }

        [Test]
        public void TestInProgressToFailedActualOwner()
        {
            IdentityId actualId = new IdentityId();
            ILoggingService loggingService = SetupLoggerMock(new List<TaskHistoryEvent>());
            Task task = new Task(
                         new TaskId(), TaskStatus.InProgress, string.Empty,
                         string.Empty, Priority.Normal, false,
                         DateTime.UtcNow, new IdentityId().GetIdentity(),
                         DateTime.UtcNow, null, actualId.GetIdentity())
            {
                LoggingService = loggingService
            };
            IPrincipal actualOwner = new ClaimsPrincipal(actualId.GetIdentity());
            Thread.CurrentPrincipal = actualOwner;
            task.Fail(new Fault());
            Assert.AreEqual(TaskStatus.Failed, task.Status);
            Assert.AreEqual(actualId.GetIdentity(), task.ActualOwner);
            Assert.IsNotNull(task.History);
            Assert.AreEqual(1, task.History.Count());
            TaskHistoryEvent history = task.History.ElementAt(0);
            Assert.IsNotNull(history);
            Assert.AreEqual(TaskStatus.InProgress, history.OldStatus);
            Assert.AreEqual(TaskStatus.Failed, history.NewStatus);
            Assert.AreEqual(actualId, history.UserId);

        }

        [Test]
        public void TestReadyToSuspendedActualOwner()
        {

            IdentityId actualId = new IdentityId();
            ILoggingService loggingService = SetupLoggerMock(new List<TaskHistoryEvent>());
            Task task = new Task(
                         new TaskId(), TaskStatus.Ready, string.Empty,
                         string.Empty, Priority.Normal, false,
                         DateTime.UtcNow, new IdentityId().GetIdentity(),
                         DateTime.UtcNow, null, actualId.GetIdentity())
            {
                LoggingService = loggingService
            };
            IPrincipal actualOwner = new ClaimsPrincipal(actualId.GetIdentity());
            Thread.CurrentPrincipal = actualOwner;
            task.Suspend();
            Assert.AreEqual(TaskStatus.Suspended, task.Status);
            Assert.AreEqual(actualId.GetIdentity(), task.ActualOwner);
            Assert.IsNotNull(task.History);
            Assert.AreEqual(1, task.History.Count());
            TaskHistoryEvent history = task.History.ElementAt(0);
            Assert.IsNotNull(history);
            Assert.AreEqual(TaskStatus.Ready, history.OldStatus);
            Assert.AreEqual(TaskStatus.Suspended, history.NewStatus);
            Assert.AreEqual(actualId, history.UserId);
            

        }
        [Test]
        public void TestReversedToSuspendedActualOwner()
        {
            IdentityId actualId = new IdentityId();
            ILoggingService loggingService = SetupLoggerMock(new List<TaskHistoryEvent>());
            Task task = new Task(
                         new TaskId(), TaskStatus.Reserved, string.Empty,
                         string.Empty, Priority.Normal, false,
                         DateTime.UtcNow, new IdentityId().GetIdentity(),
                         DateTime.UtcNow, null, actualId.GetIdentity())
            {
                LoggingService = loggingService
            };
            IPrincipal actualOwner = new ClaimsPrincipal(actualId.GetIdentity());
            Thread.CurrentPrincipal = actualOwner;
            task.Suspend();
            Assert.AreEqual(TaskStatus.Suspended, task.Status);
            Assert.AreEqual(actualId.GetIdentity(), task.ActualOwner);
            Assert.IsNotNull(task.History);
            Assert.AreEqual(1, task.History.Count());
            TaskHistoryEvent history = task.History.ElementAt(0);
            Assert.IsNotNull(history);
            Assert.AreEqual(TaskStatus.Reserved, history.OldStatus);
            Assert.AreEqual(TaskStatus.Suspended, history.NewStatus);
            Assert.AreEqual(actualId, history.UserId);
        }

        [Test]
        public void TestInProgressToSuspendedActualOwner()
        {
            IdentityId actualId = new IdentityId();
            ILoggingService loggingService = SetupLoggerMock(new List<TaskHistoryEvent>());
            Task task = new Task(
                         new TaskId(), TaskStatus.InProgress, string.Empty,
                         string.Empty, Priority.Normal, false,
                         DateTime.UtcNow, new IdentityId().GetIdentity(),
                         DateTime.UtcNow, null, actualId.GetIdentity())
            {
                LoggingService = loggingService
            };
            IPrincipal actualOwner = new ClaimsPrincipal(actualId.GetIdentity());
            Thread.CurrentPrincipal = actualOwner;
            task.Suspend();
            Assert.AreEqual(TaskStatus.Suspended, task.Status);
            Assert.AreEqual(actualId.GetIdentity(), task.ActualOwner);
            Assert.IsNotNull(task.History);
            Assert.AreEqual(1, task.History.Count());
            TaskHistoryEvent history = task.History.ElementAt(0);
            Assert.IsNotNull(history);
            Assert.AreEqual(TaskStatus.InProgress, history.OldStatus);
            Assert.AreEqual(TaskStatus.Suspended, history.NewStatus);
            Assert.AreEqual(actualId, history.UserId);
        }

        [Test]
        public void TestSuspendedToReadyActualOwner()
        {
            IdentityId actualId = new IdentityId();
            ILoggingService loggingService = SetupLoggerMock(new List<TaskHistoryEvent>());
            Task task = new Task(
                         new TaskId(), TaskStatus.Ready, string.Empty,
                         string.Empty, Priority.Normal, false,
                         DateTime.UtcNow, new IdentityId().GetIdentity(),
                         DateTime.UtcNow, null, actualId.GetIdentity())
            {
                LoggingService = loggingService
            };
            IPrincipal actualOwner = new ClaimsPrincipal(actualId.GetIdentity());
            Thread.CurrentPrincipal = actualOwner;
            task.Suspend();
            Assert.AreEqual(TaskStatus.Suspended, task.Status);
            Assert.AreEqual(actualId.GetIdentity(), task.ActualOwner);
            task.Resume();
            Assert.AreEqual(TaskStatus.Ready, task.Status);
            Assert.AreEqual(actualId.GetIdentity(), task.ActualOwner);

            Assert.IsNotNull(task.History);
            Assert.AreEqual(2, task.History.Count());
            TaskHistoryEvent history = task.History.ElementAt(0);
            Assert.IsNotNull(history);
            Assert.AreEqual(TaskStatus.Ready, history.OldStatus);
            Assert.AreEqual(TaskStatus.Suspended, history.NewStatus);
            Assert.AreEqual(actualId, history.UserId);
            history = task.History.ElementAt(1);
            Assert.IsNotNull(history);
            Assert.AreEqual(TaskStatus.Suspended, history.OldStatus);
            Assert.AreEqual(TaskStatus.Ready, history.NewStatus);
            Assert.AreEqual(actualId, history.UserId);
        }

        [Test]
        public void TestSuspendedToReversedActualOwner()
        {
            IdentityId actualId = new IdentityId();
            ILoggingService loggingService = SetupLoggerMock(new List<TaskHistoryEvent>());
            Task task = new Task(
                         new TaskId(), TaskStatus.Reserved, string.Empty,
                         string.Empty, Priority.Normal, false,
                         DateTime.UtcNow, new IdentityId().GetIdentity(),
                         DateTime.UtcNow, null, actualId.GetIdentity())
            {
                LoggingService = loggingService
            };
            IPrincipal actualOwner = new ClaimsPrincipal(actualId.GetIdentity());
            Thread.CurrentPrincipal = actualOwner;
            task.Suspend();
            Assert.AreEqual(TaskStatus.Suspended, task.Status);
            Assert.AreEqual(actualId.GetIdentity(), task.ActualOwner);
            task.Resume();
            Assert.AreEqual(TaskStatus.Reserved, task.Status);
            Assert.AreEqual(actualId.GetIdentity(), task.ActualOwner);

            Assert.IsNotNull(task.History);
            Assert.AreEqual(2, task.History.Count());
            TaskHistoryEvent history = task.History.ElementAt(0);
            Assert.IsNotNull(history);
            Assert.AreEqual(TaskStatus.Reserved, history.OldStatus);
            Assert.AreEqual(TaskStatus.Suspended, history.NewStatus);
            Assert.AreEqual(actualId, history.UserId);
            history = task.History.ElementAt(1);
            Assert.IsNotNull(history);
            Assert.AreEqual(TaskStatus.Suspended, history.OldStatus);
            Assert.AreEqual(TaskStatus.Reserved, history.NewStatus);
            Assert.AreEqual(actualId, history.UserId);

        }


        [Test]
        public void TestSuspendedToInProgressActualOwner()
        {
            IdentityId actualId = new IdentityId();
            ILoggingService loggingService = SetupLoggerMock(new List<TaskHistoryEvent>());
            Task task = new Task(
                         new TaskId(), TaskStatus.InProgress, string.Empty,
                         string.Empty, Priority.Normal, false,
                         DateTime.UtcNow, new IdentityId().GetIdentity(),
                         DateTime.UtcNow, null, actualId.GetIdentity())
            {
                LoggingService = loggingService
            };
            IPrincipal actualOwner = new ClaimsPrincipal(actualId.GetIdentity());
            Thread.CurrentPrincipal = actualOwner;
            task.Suspend();
            Assert.AreEqual(TaskStatus.Suspended, task.Status);
            Assert.AreEqual(actualId.GetIdentity(), task.ActualOwner);
            task.Resume();
            Assert.AreEqual(TaskStatus.InProgress, task.Status);
            Assert.AreEqual(actualId.GetIdentity(), task.ActualOwner);
            Assert.IsNotNull(task.History);
            Assert.AreEqual(2, task.History.Count());
            TaskHistoryEvent history = task.History.ElementAt(0);
            Assert.IsNotNull(history);
            Assert.AreEqual(TaskStatus.InProgress, history.OldStatus);
            Assert.AreEqual(TaskStatus.Suspended, history.NewStatus);
            Assert.AreEqual(actualId, history.UserId);
            history = task.History.ElementAt(1);
            Assert.IsNotNull(history);
            Assert.AreEqual(TaskStatus.Suspended, history.OldStatus);
            Assert.AreEqual(TaskStatus.InProgress, history.NewStatus);
            Assert.AreEqual(actualId, history.UserId);

        }

        [Test]
        public void TestStopActualOwner()
        {
            IdentityId actualId = new IdentityId();
            ILoggingService loggingService = SetupLoggerMock(new List<TaskHistoryEvent>());
            Task task = new Task(
                         new TaskId(), TaskStatus.InProgress, string.Empty,
                         string.Empty, Priority.Normal, false,
                         DateTime.UtcNow, new IdentityId().GetIdentity(),
                         DateTime.UtcNow, null, actualId.GetIdentity())
            {
                LoggingService = loggingService
            };
            IPrincipal actualOwner = new ClaimsPrincipal(actualId.GetIdentity());
            Thread.CurrentPrincipal = actualOwner;
            task.Stop();
            Assert.AreEqual(TaskStatus.Reserved, task.Status);
            Assert.AreEqual(actualId.GetIdentity(), task.ActualOwner);
            Assert.IsNotNull(task.History);
            Assert.AreEqual(1, task.History.Count());
            TaskHistoryEvent history = task.History.ElementAt(0);
            Assert.IsNotNull(history);
            Assert.AreEqual(TaskStatus.InProgress, history.OldStatus);
            Assert.AreEqual(TaskStatus.Reserved, history.NewStatus);
            Assert.AreEqual(actualId, history.UserId);

        }

        [Test]
        public void TestDelegateActualOwner()
        {
            IdentityId potentialOwner=new IdentityId();
            IdentityId actualId = new IdentityId();
            _mapStorage[potentialOwner.Id] = new Account(potentialOwner, "s-1-2-3-4-5", "auth", AccountType.User);

            ILoggingService loggingService = SetupLoggerMock(new List<TaskHistoryEvent>());
            Task task = new Task(
                         new TaskId(), TaskStatus.InProgress, string.Empty,
                         string.Empty, Priority.Normal, false,
                         DateTime.UtcNow, new IdentityId().GetIdentity(),
                         DateTime.UtcNow, null, actualId.GetIdentity())
            {
                LoggingService = loggingService
            };
            IPrincipal actualOwner = new ClaimsPrincipal(actualId.GetIdentity());
            Thread.CurrentPrincipal = actualOwner;

            task.Delegate(potentialOwner.GetIdentity(), Priority.High);
            Assert.AreEqual(TaskStatus.Reserved, task.Status);
            Assert.IsNotNull(task.ActualOwner);
            Assert.AreEqual(potentialOwner, task.ActualOwner.GetMappedId());
            Assert.AreEqual(Priority.High, task.Priority);

            Assert.IsNotNull(task.History);
            Assert.AreEqual(1, task.History.Count());
            TaskHistoryEvent history = task.History.ElementAt(0);
            Assert.IsNotNull(history);
            Assert.AreEqual(TaskStatus.InProgress, history.OldStatus);
            Assert.AreEqual(TaskStatus.Reserved, history.NewStatus);
            Assert.AreEqual(Priority.Normal,history.OldPriority);
            Assert.AreEqual(Priority.High, history.NewPriority);
            Assert.AreEqual(actualId, history.UserId);

        }

        [Test]
        public void TestRevokeActualOwner()
        {
            IdentityId actualId = new IdentityId();
            ILoggingService loggingService = SetupLoggerMock(new List<TaskHistoryEvent>());
            Task task = new Task(
                         new TaskId(), TaskStatus.Reserved, string.Empty,
                         string.Empty, Priority.Normal, false,
                         DateTime.UtcNow, new IdentityId().GetIdentity(),
                         DateTime.UtcNow, null, actualId.GetIdentity())
            {
                LoggingService = loggingService
            };
            IPrincipal actualOwner = new ClaimsPrincipal(actualId.GetIdentity());
            Thread.CurrentPrincipal = actualOwner;
            task.Release();
            Assert.AreEqual(TaskStatus.Ready, task.Status);
            Assert.IsNull(task.ActualOwner);

            Assert.IsNotNull(task.History);
            Assert.AreEqual(1, task.History.Count());
            TaskHistoryEvent history = task.History.ElementAt(0);
            Assert.IsNotNull(history);
            Assert.AreEqual(TaskStatus.Reserved, history.OldStatus);
            Assert.AreEqual(TaskStatus.Ready, history.NewStatus);
            Assert.AreEqual(actualId, history.UserId);
            Assert.AreEqual(actualId,history.StartOwner);
            Assert.IsNull(history.EndOwner);

        }

        [Test]
        public void TestInProgressForwardActualOwner()
        {
            IdentityId potentialOwner = new IdentityId();
            IdentityId actualId = new IdentityId();
            _mapStorage[potentialOwner.Id] = new Account(potentialOwner, "s-1-2-3-4-5", "auth", AccountType.User);

            ILoggingService loggingService = SetupLoggerMock(new List<TaskHistoryEvent>());
            Task task = new Task(
                         new TaskId(), TaskStatus.InProgress, string.Empty,
                         string.Empty, Priority.Normal, false,
                         DateTime.UtcNow, new IdentityId().GetIdentity(),
                         DateTime.UtcNow, null, actualId.GetIdentity())
            {
                LoggingService = loggingService
            };
            IPrincipal actualOwner = new ClaimsPrincipal(actualId.GetIdentity());
            Thread.CurrentPrincipal = actualOwner;

            task.Forward(potentialOwner.GetIdentity());
            Assert.AreEqual(TaskStatus.Ready, task.Status);
            Assert.IsNotNull(task.ActualOwner);
            Assert.AreEqual(potentialOwner, task.ActualOwner.GetMappedId());

            Assert.IsNotNull(task.History);
            Assert.AreEqual(1, task.History.Count());
            TaskHistoryEvent history = task.History.ElementAt(0);
            Assert.IsNotNull(history);
            Assert.AreEqual(TaskStatus.InProgress, history.OldStatus);
            Assert.AreEqual(TaskStatus.Ready, history.NewStatus);
            Assert.AreEqual(actualId, history.UserId);
            Assert.AreEqual(actualId, history.StartOwner);
            Assert.AreEqual(potentialOwner, history.EndOwner);

        }
        [Test]
        public void TestReservedForwardActualOwner()
        {
            IdentityId potentialOwner = new IdentityId();
            IdentityId actualId = new IdentityId();
            _mapStorage[potentialOwner.Id] = new Account(potentialOwner, "s-1-2-3-4-5", "auth", AccountType.User);

            ILoggingService loggingService = SetupLoggerMock(new List<TaskHistoryEvent>());
            Task task = new Task(
                         new TaskId(), TaskStatus.Reserved, string.Empty,
                         string.Empty, Priority.Normal, false,
                         DateTime.UtcNow, new IdentityId().GetIdentity(),
                         DateTime.UtcNow, null, actualId.GetIdentity())
            {
                LoggingService = loggingService
            };
            IPrincipal actualOwner = new ClaimsPrincipal(actualId.GetIdentity());
            Thread.CurrentPrincipal = actualOwner;

            task.Forward(potentialOwner.GetIdentity());
            Assert.AreEqual(TaskStatus.Ready, task.Status);
            Assert.IsNotNull(task.ActualOwner);
            Assert.AreEqual(potentialOwner, task.ActualOwner.GetMappedId());

            Assert.IsNotNull(task.History);
            Assert.AreEqual(1, task.History.Count());
            TaskHistoryEvent history = task.History.ElementAt(0);
            Assert.IsNotNull(history);
            Assert.AreEqual(TaskStatus.Reserved, history.OldStatus);
            Assert.AreEqual(TaskStatus.Ready, history.NewStatus);
            Assert.AreEqual(actualId, history.UserId);
            Assert.AreEqual(actualId, history.StartOwner);
            Assert.AreEqual(potentialOwner, history.EndOwner);
        }

        [Test]
        public void TestReadyForwardActualOwner()
        {
            IdentityId potentialOwner = new IdentityId();
            IdentityId actualId = new IdentityId();
            _mapStorage[potentialOwner.Id] = new Account(potentialOwner, "s-1-2-3-4-5", "auth", AccountType.User);

            ILoggingService loggingService = SetupLoggerMock(new List<TaskHistoryEvent>());
            Task task = new Task(
                         new TaskId(), TaskStatus.Ready, string.Empty,
                         string.Empty, Priority.Normal, false,
                         DateTime.UtcNow, new IdentityId().GetIdentity(),
                         DateTime.UtcNow, null, actualId.GetIdentity())
            {
                LoggingService = loggingService
            };
            IPrincipal actualOwner = new ClaimsPrincipal(actualId.GetIdentity());
            Thread.CurrentPrincipal = actualOwner;          

            task.Forward(potentialOwner.GetIdentity());
            Assert.AreEqual(TaskStatus.Ready, task.Status);
            Assert.IsNotNull(task.ActualOwner);
            Assert.AreEqual(potentialOwner, task.ActualOwner.GetMappedId());
            Assert.IsTrue(task.PotentialOwners.Contains(potentialOwner.GetIdentity()));

            Assert.IsNotNull(task.History);
            Assert.AreEqual(1, task.History.Count());
            TaskHistoryEvent history = task.History.ElementAt(0);
            Assert.IsNotNull(history);
            Assert.AreEqual(TaskStatus.Ready, history.OldStatus);
            Assert.AreEqual(TaskStatus.Ready, history.NewStatus);
            Assert.AreEqual(actualId, history.UserId);
            Assert.AreEqual(actualId, history.StartOwner);
            Assert.AreEqual(potentialOwner, history.EndOwner);
        }

        [Test]
        public void TestNominateIndividual()
        {
            IdentityId potentialOwner = new IdentityId();
            IdentityId businessAdministrator = new IdentityId();
            _mapStorage[potentialOwner.Id] = new Account(potentialOwner, "s-1-2-3-4-5", "auth", AccountType.User);

            ILoggingService loggingService = SetupLoggerMock(new List<TaskHistoryEvent>());
            Task task = new Task(
                         new TaskId(), TaskStatus.Created, string.Empty,
                         string.Empty, Priority.Normal, false,
                         DateTime.UtcNow, new IdentityId().GetIdentity(),
                         DateTime.UtcNow, null, null)
            {
                LoggingService = loggingService
            };
            IPrincipal actualOwner = new ClaimsPrincipal(businessAdministrator.GetIdentity());
            Thread.CurrentPrincipal = actualOwner;
            task.BusinessAdministrators.Add(businessAdministrator.GetIdentity());
            task.Nominate(potentialOwner.GetIdentity());
            Assert.AreEqual(TaskStatus.Reserved, task.Status);
            Assert.IsNotNull(task.ActualOwner);
            Assert.AreEqual(potentialOwner, task.ActualOwner.GetMappedId());
            Assert.IsTrue(task.PotentialOwners.Contains(potentialOwner.GetIdentity()));
            Assert.IsNotNull(task.History);
            Assert.AreEqual(1, task.History.Count());
            TaskHistoryEvent history = task.History.ElementAt(0);
            Assert.IsNotNull(history);
            Assert.AreEqual(TaskStatus.Created, history.OldStatus);
            Assert.AreEqual(TaskStatus.Reserved, history.NewStatus);
            Assert.AreEqual(businessAdministrator, history.UserId);
            Assert.AreEqual(null,history.StartOwner);
            Assert.AreEqual(potentialOwner, history.EndOwner);
        }

        [Test]
        public void TestNominateGroup()
        {
            IdentityId potentialOwner = new IdentityId();
            IdentityId businessAdministrator = new IdentityId();
            _mapStorage[potentialOwner.Id] = new Account(potentialOwner, "s-1-2-3-4-5", "auth", AccountType.Group);

            ILoggingService loggingService = SetupLoggerMock(new List<TaskHistoryEvent>());
            Task task = new Task(
                         new TaskId(), TaskStatus.Created, string.Empty,
                         string.Empty, Priority.Normal, false,
                         DateTime.UtcNow, new IdentityId().GetIdentity(),
                         DateTime.UtcNow, null, null)
            {
                LoggingService = loggingService
            };
            IPrincipal actualOwner = new ClaimsPrincipal(businessAdministrator.GetIdentity());
            Thread.CurrentPrincipal = actualOwner;
            task.BusinessAdministrators.Add(businessAdministrator.GetIdentity());
            task.Nominate(potentialOwner.GetIdentity());
            Assert.AreEqual(TaskStatus.Ready, task.Status);
            Assert.IsNull(task.ActualOwner);
            Assert.IsTrue(task.PotentialOwners.Contains(potentialOwner.GetIdentity()));
            

            Assert.IsNotNull(task.History);
            Assert.AreEqual(1, task.History.Count());
            TaskHistoryEvent history = task.History.ElementAt(0);
            Assert.IsNotNull(history);
            Assert.AreEqual(TaskStatus.Created, history.OldStatus);
            Assert.AreEqual(TaskStatus.Ready, history.NewStatus);
            Assert.AreEqual(businessAdministrator, history.UserId);
            Assert.AreEqual(null, history.StartOwner);
            Assert.AreEqual(null, history.EndOwner);

        }


        #region Mock Account Factory
        private class MockAccountFactory : IAccountFactory
        {
            private readonly Dictionary<Guid, Account> _mapStorage = new Dictionary<Guid, Account>();

            /// <summary>
            /// Initializes a new instance of the <see cref="MockAccountFactory"/> class.
            /// </summary>
            /// <param name="mapStorage">The map storage.</param>
            public MockAccountFactory(Dictionary<Guid, Account> mapStorage)
            {
                _mapStorage = mapStorage;
            }


            public Account GetAccount(IdentityId id)
            {
                Account acc;
                if(!_mapStorage.TryGetValue(id.Id, out acc))
                    throw new AccountNotMapped();
                return acc;
            }

            public IEnumerable<Account> GetRoles(IdentityId id)
            {
                return new Account[] {};
            }

            public IEnumerable<Account> GetGroups(IdentityId id)
            {
                return new Account[] { };
            }

            public IdentityId GetMappedAccountId(string authenticationType, string nativeId)
            {
                return _mapStorage.Values.Single(a => a.NativeId == nativeId).MappedId;
            }
        }
        #endregion
    }
}
