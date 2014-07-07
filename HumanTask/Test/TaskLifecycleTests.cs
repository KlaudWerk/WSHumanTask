using System;
using System.Collections.Generic;
using System.Security.Principal;
using KlaudWerk.Security;
using NUnit.Framework;

namespace HumanTask.Test
{
    [TestFixture]
    public class TaskLifecycleTests:TaskMocksSetup
    {
        #region Task Life cycle Operations
        /// <summary>
        /// Tests Claim Task lifecycle operation.
        /// Initial status must be 'Ready'
        /// The calling proncipal must be in Potential Owner or Business Administrator role
        /// </summary>
        [Test]
        public void TestClaimReadyStatus()
        {
            string role = string.Empty;
            IPrincipal initiator = SetupMockPrincipal("Initiator",
                new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.Initiator.ToString().Equals(role));
            IPrincipal caller = SetupMockPrincipal("Caller",
                new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.PotentialOwner.ToString().Equals(role) 
                    || HumanRoles.BusinessAdministrator.ToString().Equals(role));

            TaskEntity task=new TaskEntity{TaskId = new TaskId(),Status = TaskStatus.Ready,Initiator = initiator.Identity};
            TaskLifecycle lifecycle=new TaskLifecycle();
            lifecycle.Claim(task,caller);
            Assert.AreEqual(TaskStatus.Reserved,task.Status);
            Assert.IsNotNull(task.ActualOwner);
            //Assert.AreEqual(initiator.Identity,task.ActualOwner);
        }

        /// <summary>
        /// Tests Claim Task lifecycle operation.
        /// If the status is not 'Ready', 
        /// an instance of <see cref="TaskInvalidStateException"/> must be thrown
        /// </summary>
        [Test]
        [ExpectedException(typeof(TaskInvalidStateException))]
        public void TestClaimInvalidStatus()
        {
            string role = string.Empty;
            IPrincipal initiator = SetupMockPrincipal("Initiator",
                new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.Initiator.ToString().Equals(role));
            IPrincipal caller = SetupMockPrincipal("Caller",
                new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.PotentialOwner.ToString().Equals(role)
                    || HumanRoles.BusinessAdministrator.ToString().Equals(role));

            TaskEntity task = new TaskEntity { TaskId = new TaskId(), Status = TaskStatus.Reserved, Initiator = initiator.Identity };
            TaskLifecycle lifecycle = new TaskLifecycle();
            lifecycle.Claim(task, caller);
        }

        /// <summary>
        /// Tests the claim invalid principal role.
        /// If the calling proncipal not in Potential Owner or Business Administrator role, 
        /// an instance of <see cref="TaskAccessException"/>  must be thrown      
        /// </summary>
        [Test]
        [ExpectedException(typeof(TaskAccessException))]
        public void TestClaimInvalidPrincipalRole()
        {
            string role = string.Empty;
            IPrincipal initiator = SetupMockPrincipal("Initiator",
                new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.Initiator.ToString().Equals(role));
            IPrincipal caller = SetupMockPrincipal("Caller",
                new IdentityId(), 
                s => { role = s; },
                () => false);

            TaskEntity task = new TaskEntity { TaskId = new TaskId(), Status = TaskStatus.Ready, 
                Initiator = initiator.Identity };
            TaskLifecycle lifecycle = new TaskLifecycle();
            lifecycle.Claim(task, caller);
        }

        [Test]
        public void TestStartFromReserved()
        {
            string role = string.Empty;
            IPrincipal initiator = SetupMockPrincipal("Initiator",
                new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.Initiator.ToString().Equals(role));
            IPrincipal caller = SetupMockPrincipal("Caller",
                new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.ActualOwner.ToString().Equals(role));

            TaskEntity task = new TaskEntity
            {
                TaskId = new TaskId(),
                Status = TaskStatus.Reserved,
                Initiator = initiator.Identity,
                ActualOwner = caller.Identity
            };
            TaskLifecycle lifecycle = new TaskLifecycle();
            lifecycle.Start(task, caller);
            Assert.AreEqual(TaskStatus.InProgress,task.Status);
            Assert.IsNotNull(task.Started);
        }

        [Test]
        public void TestStartFromReady()
        {
            string role = string.Empty;
            IPrincipal initiator = SetupMockPrincipal("Initiator",
                new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.Initiator.ToString().Equals(role));
            IPrincipal caller = SetupMockPrincipal("Caller",
                new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.PotentialOwner.ToString().Equals(role));

            TaskEntity task = new TaskEntity
            {
                TaskId = new TaskId(),
                Status = TaskStatus.Ready,
                Initiator = initiator.Identity,
                ActualOwner = caller.Identity
            };
            TaskLifecycle lifecycle = new TaskLifecycle();
            lifecycle.Start(task, caller);
            Assert.AreEqual(TaskStatus.InProgress, task.Status);
            Assert.IsNotNull(task.Started);
        }

        [Test]
        [ExpectedException(typeof(TaskInvalidStateException))]
        public void TestStartInvalidStatus()
        {
            string role = string.Empty;
            IPrincipal initiator = SetupMockPrincipal("Initiator",
                new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.Initiator.ToString().Equals(role));
            IPrincipal caller = SetupMockPrincipal("Caller",
                new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.PotentialOwner.ToString().Equals(role));

            TaskEntity task = new TaskEntity
            {
                TaskId = new TaskId(),
                Status = TaskStatus.InProgress,
                Initiator = initiator.Identity,
                ActualOwner = caller.Identity
            };
            TaskLifecycle lifecycle = new TaskLifecycle();
            lifecycle.Start(task, caller);
        }

        [Test]
        [ExpectedException(typeof(TaskAccessException))]
        public void TestStartFromReadyInvalidRole()
        {
            string role = string.Empty;
            IPrincipal initiator = SetupMockPrincipal("Initiator",
                new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.Initiator.ToString().Equals(role));
            IPrincipal caller = SetupMockPrincipal("Caller",
                new IdentityId(), 
                s => { role = s; },
                () => false);

            TaskEntity task = new TaskEntity
            {
                TaskId = new TaskId(),
                Status = TaskStatus.Ready,
                Initiator = initiator.Identity,
                ActualOwner = caller.Identity
            };
            TaskLifecycle lifecycle = new TaskLifecycle();
            lifecycle.Start(task, caller);

        }

        [Test]
        [ExpectedException(typeof(TaskAccessException))]
        public void TestStartFromReservedInvalidRole()
        {
            string role = string.Empty;
            IPrincipal initiator = SetupMockPrincipal("Initiator",
                new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.Initiator.ToString().Equals(role));
            IPrincipal caller = SetupMockPrincipal("Caller",
                new IdentityId(), 
                s => { role = s; },
                () => false);

            TaskEntity task = new TaskEntity
            {
                TaskId = new TaskId(),
                Status = TaskStatus.Reserved,
                Initiator = initiator.Identity,
                ActualOwner = caller.Identity
            };
            TaskLifecycle lifecycle = new TaskLifecycle();
            lifecycle.Start(task, caller);

        }


        [Test]
        public void TestStop()
        {
            string role = string.Empty;
            IPrincipal initiator = SetupMockPrincipal("Initiator",
                new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.Initiator.ToString().Equals(role));
            IPrincipal caller = SetupMockPrincipal("Caller",
                new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.ActualOwner.ToString().Equals(role)
                    ||HumanRoles.BusinessAdministrator.ToString().Equals(role));

            TaskEntity task = new TaskEntity
            {
                TaskId = new TaskId(),
                Status = TaskStatus.InProgress,
                Initiator = initiator.Identity,
                ActualOwner = caller.Identity
            };
            TaskLifecycle lifecycle = new TaskLifecycle();
            lifecycle.Stop(task, caller);
            Assert.AreEqual(TaskStatus.Reserved,task.Status);
        }

        [Test]
        [ExpectedException(typeof(TaskInvalidStateException))]
        public void TestStopInvalidState()
        {
            string role = string.Empty;
            IPrincipal initiator = SetupMockPrincipal("Initiator",
                new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.Initiator.ToString().Equals(role));
            IPrincipal caller = SetupMockPrincipal("Caller",
                new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.ActualOwner.ToString().Equals(role)
                    || HumanRoles.BusinessAdministrator.ToString().Equals(role));

            TaskEntity task = new TaskEntity
            {
                TaskId = new TaskId(),
                Status = TaskStatus.Created,
                Initiator = initiator.Identity,
                ActualOwner = caller.Identity
            };
            TaskLifecycle lifecycle = new TaskLifecycle();
            lifecycle.Stop(task, caller);
        }

        [Test]
        [ExpectedException(typeof(TaskAccessException))]
        public void TestStopInvalidRole()
        {
            string role = string.Empty;
            IPrincipal initiator = SetupMockPrincipal("Initiator",
                new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.Initiator.ToString().Equals(role));
            IPrincipal caller = SetupMockPrincipal("Caller",
                new IdentityId(), 
                s => { role = s; },
                () => false);

            TaskEntity task = new TaskEntity
            {
                TaskId = new TaskId(),
                Status = TaskStatus.InProgress,
                Initiator = initiator.Identity,
                ActualOwner = caller.Identity
            };
            TaskLifecycle lifecycle = new TaskLifecycle();
            lifecycle.Stop(task, caller);
        }


        [Test]
        public void TestRelease()
        {
            string role = string.Empty;
            IPrincipal initiator = SetupMockPrincipal("Initiator",
                new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.Initiator.ToString().Equals(role));
            IPrincipal caller = SetupMockPrincipal("Caller",
                new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.ActualOwner.ToString().Equals(role)
                    || HumanRoles.BusinessAdministrator.ToString().Equals(role));

            TaskEntity task = new TaskEntity
            {
                TaskId = new TaskId(),
                Status = TaskStatus.Reserved,
                Initiator = initiator.Identity,
                ActualOwner = caller.Identity
            };
            TaskLifecycle lifecycle = new TaskLifecycle();
            lifecycle.Release(task, caller);
            Assert.AreEqual(TaskStatus.Ready,task.Status);
            Assert.IsNull(task.ActualOwner);
        }

        [Test]
        [ExpectedException(typeof(TaskInvalidStateException))]
        public void TestReleaseInvalidState()
        {
            string role = string.Empty;
            IPrincipal initiator = SetupMockPrincipal("Initiator",
                new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.Initiator.ToString().Equals(role));
            IPrincipal caller = SetupMockPrincipal("Caller",
                new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.ActualOwner.ToString().Equals(role)
                    || HumanRoles.BusinessAdministrator.ToString().Equals(role));

            TaskEntity task = new TaskEntity
            {
                TaskId = new TaskId(),
                Status = TaskStatus.Ready,
                Initiator = initiator.Identity,
                ActualOwner = caller.Identity
            };
            TaskLifecycle lifecycle = new TaskLifecycle();
            lifecycle.Release(task, caller);
        }
        [Test]
        [ExpectedException(typeof(TaskAccessException))]
        public void TestReleaseInvalidRole()
        {
            string role = string.Empty;
            IPrincipal initiator = SetupMockPrincipal("Initiator",
                new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.Initiator.ToString().Equals(role));
            IPrincipal caller = SetupMockPrincipal("Caller",
                new IdentityId(), 
                s => { role = s; },
                () => false);

            TaskEntity task = new TaskEntity
            {
                TaskId = new TaskId(),
                Status = TaskStatus.Reserved,
                Initiator = initiator.Identity,
                ActualOwner = caller.Identity
            };
            TaskLifecycle lifecycle = new TaskLifecycle();
            lifecycle.Release(task, caller);
        }

        [Test]
        public void TestSuspend()
        {
            string role = string.Empty;
            IPrincipal initiator = SetupMockPrincipal("Initiator",
                new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.Initiator.ToString().Equals(role));
            IPrincipal caller = SetupMockPrincipal("Caller",
                new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.ActualOwner.ToString().Equals(role)
                    || HumanRoles.PotentialOwner.ToString().Equals(role));

            TaskEntity task = new TaskEntity
            {
                TaskId = new TaskId(),
                Status = TaskStatus.Ready,
                Initiator = initiator.Identity,
                ActualOwner = caller.Identity
            };
            TaskLifecycle lifecycle = new TaskLifecycle();
            lifecycle.Suspend(task,caller);
            Assert.AreEqual(TaskStatus.Suspended,task.Status);
            Assert.IsNotNull(task.SuspendedState);
            Assert.AreEqual(TaskStatus.Ready,task.SuspendedState.OriginalStatus);
            Assert.AreEqual(DateTime.MaxValue,task.SuspendedState.SuspensionEnds);
        }
        [Test]
        public void TestSuspendUntilWithDate()
        {
            string role = string.Empty;
            IPrincipal initiator = SetupMockPrincipal("Initiator",
                new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.Initiator.ToString().Equals(role));
            IPrincipal caller = SetupMockPrincipal("Caller",
                new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.ActualOwner.ToString().Equals(role)
                    || HumanRoles.BusinessAdministrator.ToString().Equals(role));

            TaskEntity task = new TaskEntity
            {
                TaskId = new TaskId(),
                Status = TaskStatus.Ready,
                Initiator = initiator.Identity,
                ActualOwner = caller.Identity
            };
            TaskLifecycle lifecycle = new TaskLifecycle();
            DateTime suspensionEnds = DateTime.UtcNow + new TimeSpan(1, 0, 0, 0);
            lifecycle.SuspendUntil(task, suspensionEnds, caller);
            Assert.AreEqual(TaskStatus.Suspended, task.Status);
            Assert.IsNotNull(task.SuspendedState);
            Assert.AreEqual(TaskStatus.Ready, task.SuspendedState.OriginalStatus);
            Assert.AreEqual(suspensionEnds, task.SuspendedState.SuspensionEnds);
            
        }
        [Test]
        public void TestSuspendUntilWithTimeSpan()
        {
            string role = string.Empty;
            IPrincipal initiator = SetupMockPrincipal("Initiator",
                new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.Initiator.ToString().Equals(role));
            IPrincipal caller = SetupMockPrincipal("Caller",
                new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.ActualOwner.ToString().Equals(role)
                    || HumanRoles.BusinessAdministrator.ToString().Equals(role));

            TaskEntity task = new TaskEntity
            {
                TaskId = new TaskId(),
                Status = TaskStatus.Ready,
                Initiator = initiator.Identity,
                ActualOwner = caller.Identity
            };
            TaskLifecycle lifecycle = new TaskLifecycle();
            TimeSpan ts = new TimeSpan(1, 10, 0, 0);
            lifecycle.SuspendUntil(task, ts, caller);
            Assert.AreEqual(TaskStatus.Suspended, task.Status);
            Assert.IsNotNull(task.SuspendedState);
            Assert.AreEqual(TaskStatus.Ready, task.SuspendedState.OriginalStatus);
            TimeSpan diff = task.SuspendedState.SuspensionEnds - DateTime.UtcNow;
            Assert.AreEqual(1,diff.Days);
            Assert.Greater(diff.Minutes,0);
        }
        [Test]
        public void TestSuspendUntilWithDateFromReservedState()
        {
            string role = string.Empty;
            IPrincipal initiator = SetupMockPrincipal("Initiator",
                new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.Initiator.ToString().Equals(role));
            IPrincipal caller = SetupMockPrincipal("Caller",
                new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.ActualOwner.ToString().Equals(role)
                    || HumanRoles.BusinessAdministrator.ToString().Equals(role));

            TaskEntity task = new TaskEntity
            {
                TaskId = new TaskId(),
                Status = TaskStatus.Reserved,
                Initiator = initiator.Identity,
                ActualOwner = caller.Identity
            };
            TaskLifecycle lifecycle = new TaskLifecycle();
            DateTime suspensionEnds = DateTime.UtcNow + new TimeSpan(1, 0, 0, 0);
            lifecycle.SuspendUntil(task, suspensionEnds, caller);
            Assert.AreEqual(TaskStatus.Suspended, task.Status);
            Assert.IsNotNull(task.SuspendedState);
            Assert.AreEqual(TaskStatus.Reserved, task.SuspendedState.OriginalStatus);
            Assert.AreEqual(suspensionEnds, task.SuspendedState.SuspensionEnds);
        }
        [Test]
        public void TestSuspendUntilWithDateFromInProgressState()
        {
            string role = string.Empty;
            IPrincipal initiator = SetupMockPrincipal("Initiator",
                new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.Initiator.ToString().Equals(role));
            IPrincipal caller = SetupMockPrincipal("Caller",
                new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.ActualOwner.ToString().Equals(role)
                    || HumanRoles.BusinessAdministrator.ToString().Equals(role));

            TaskEntity task = new TaskEntity
            {
                TaskId = new TaskId(),
                Status = TaskStatus.InProgress,
                Initiator = initiator.Identity,
                ActualOwner = caller.Identity
            };
            TaskLifecycle lifecycle = new TaskLifecycle();
            DateTime suspensionEnds = DateTime.UtcNow + new TimeSpan(1, 0, 0, 0);
            lifecycle.SuspendUntil(task, suspensionEnds, caller);
            Assert.AreEqual(TaskStatus.Suspended, task.Status);
            Assert.IsNotNull(task.SuspendedState);
            Assert.AreEqual(TaskStatus.InProgress, task.SuspendedState.OriginalStatus);
            Assert.AreEqual(suspensionEnds, task.SuspendedState.SuspensionEnds);
        }

        [Test]
        [ExpectedException(typeof(TaskInvalidStateException))]
        public void TestSuspendUntilWithDateInvalidState()
        {
            string role = string.Empty;
            IPrincipal initiator = SetupMockPrincipal("Initiator",
                new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.Initiator.ToString().Equals(role));
            IPrincipal caller = SetupMockPrincipal("Caller",
                new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.ActualOwner.ToString().Equals(role)
                    || HumanRoles.BusinessAdministrator.ToString().Equals(role));

            TaskEntity task = new TaskEntity
            {
                TaskId = new TaskId(),
                Status = TaskStatus.Created,
                Initiator = initiator.Identity,
                ActualOwner = caller.Identity
            };
            TaskLifecycle lifecycle = new TaskLifecycle();
            DateTime suspensionEnds = DateTime.UtcNow + new TimeSpan(1, 0, 0, 0);
            lifecycle.SuspendUntil(task, suspensionEnds, caller);
        }

        [Test]
        [ExpectedException(typeof(TaskAccessException))]
        public void TestSuspendUntilWithDateFromReadyStateInvalidRole()
        {
            string role = string.Empty;
            IPrincipal initiator = SetupMockPrincipal("Initiator",
                new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.Initiator.ToString().Equals(role));
            IPrincipal caller = SetupMockPrincipal("Caller",
                new IdentityId(), 
                s => { role = s; },
                () => false);

            TaskEntity task = new TaskEntity
            {
                TaskId = new TaskId(),
                Status = TaskStatus.Ready,
                Initiator = initiator.Identity,
                ActualOwner = caller.Identity
            };
            TaskLifecycle lifecycle = new TaskLifecycle();
            DateTime suspensionEnds = DateTime.UtcNow + new TimeSpan(1, 0, 0, 0);
            lifecycle.SuspendUntil(task, suspensionEnds, caller);

        }
        [Test]
        [ExpectedException(typeof(TaskAccessException))]
        public void TestSuspendUntilWithDatefromReservedStateInvalidRole()
        {
            string role = string.Empty;
            IPrincipal initiator = SetupMockPrincipal("Initiator",
                new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.Initiator.ToString().Equals(role));
            IPrincipal caller = SetupMockPrincipal("Caller",
                new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.PotentialOwner.ToString().Equals(role));

            TaskEntity task = new TaskEntity
            {
                TaskId = new TaskId(),
                Status = TaskStatus.Reserved,
                Initiator = initiator.Identity,
                ActualOwner = caller.Identity
            };
            TaskLifecycle lifecycle = new TaskLifecycle();
            DateTime suspensionEnds = DateTime.UtcNow + new TimeSpan(1, 0, 0, 0);
            lifecycle.SuspendUntil(task, suspensionEnds, caller);
        }
        [Test]
        [ExpectedException(typeof(TaskAccessException))]
        public void TestSuspendUntilWithDateFromInProgressStateInvalidRole()
        {
            string role = string.Empty;
            IPrincipal initiator = SetupMockPrincipal("Initiator",
                new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.Initiator.ToString().Equals(role));
            IPrincipal caller = SetupMockPrincipal("Caller",
                new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.PotentialOwner.ToString().Equals(role));

            TaskEntity task = new TaskEntity
            {
                TaskId = new TaskId(),
                Status = TaskStatus.InProgress,
                Initiator = initiator.Identity,
                ActualOwner = caller.Identity
            };
            TaskLifecycle lifecycle = new TaskLifecycle();
            DateTime suspensionEnds = DateTime.UtcNow + new TimeSpan(1, 0, 0, 0);
            lifecycle.SuspendUntil(task, suspensionEnds, caller);
        }


        [Test]
        public void TestResume()
        {
            string role = string.Empty;
            IPrincipal initiator = SetupMockPrincipal("Initiator",
                new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.Initiator.ToString().Equals(role));
            IPrincipal caller = SetupMockPrincipal("Caller",
                new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.ActualOwner.ToString().Equals(role));

            TaskEntity task = new TaskEntity
            {
                TaskId = new TaskId(),
                Status = TaskStatus.Suspended,
                SuspendedState = new SuspendedState{OriginalStatus = TaskStatus.InProgress},
                Initiator = initiator.Identity,
                ActualOwner = caller.Identity
            };
            TaskLifecycle lifecycle = new TaskLifecycle();
            lifecycle.Resume(task, caller);
            Assert.IsNull(task.SuspendedState);
            Assert.AreEqual(TaskStatus.InProgress,task.Status);
        }

        [Test]
        public void TestResumeFromSuspendedReadyState()
        {
            string role = string.Empty;
            IPrincipal initiator = SetupMockPrincipal("Initiator",
                new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.Initiator.ToString().Equals(role));
            IPrincipal caller = SetupMockPrincipal("Caller",
                new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.PotentialOwner.ToString().Equals(role));

            TaskEntity task = new TaskEntity
            {
                TaskId = new TaskId(),
                Status = TaskStatus.Suspended,
                SuspendedState = new SuspendedState { OriginalStatus = TaskStatus.Ready },
                Initiator = initiator.Identity,
                ActualOwner = caller.Identity
            };
            TaskLifecycle lifecycle = new TaskLifecycle();
            lifecycle.Resume(task, caller);
            Assert.IsNull(task.SuspendedState);
            Assert.AreEqual(TaskStatus.Ready, task.Status);
        }
        

        [Test]
        [ExpectedException(typeof(TaskInvalidStateException))]
        public void TestResumeNoSavedSuspendState()
        {
            string role = string.Empty;
            IPrincipal initiator = SetupMockPrincipal("Initiator",
                new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.Initiator.ToString().Equals(role));
            IPrincipal caller = SetupMockPrincipal("Caller",
                new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.PotentialOwner.ToString().Equals(role));

            TaskEntity task = new TaskEntity
            {
                TaskId = new TaskId(),
                Status = TaskStatus.Suspended,
                Initiator = initiator.Identity,
                ActualOwner = caller.Identity
            };
            TaskLifecycle lifecycle = new TaskLifecycle();
            lifecycle.Resume(task, caller);
        }

        [Test]
        [ExpectedException(typeof(TaskInvalidStateException))]
        public void TestResumeInvalidState()
        {
            string role = string.Empty;
            IPrincipal initiator = SetupMockPrincipal("Initiator",
                new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.Initiator.ToString().Equals(role));
            IPrincipal caller = SetupMockPrincipal("Caller",
                new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.PotentialOwner.ToString().Equals(role));

            TaskEntity task = new TaskEntity
            {
                TaskId = new TaskId(),
                Status = TaskStatus.Ready,
                SuspendedState = new SuspendedState { OriginalStatus = TaskStatus.Ready },
                Initiator = initiator.Identity,
                ActualOwner = caller.Identity
            };
            TaskLifecycle lifecycle = new TaskLifecycle();
            lifecycle.Resume(task, caller);
        }

        [Test]
        [ExpectedException(typeof(TaskAccessException))]
        public void TestResumeInvalidRole()
        {
            string role = string.Empty;
            IPrincipal initiator = SetupMockPrincipal("Initiator",
                new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.Initiator.ToString().Equals(role));
            IPrincipal caller = SetupMockPrincipal("Caller",
                new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.PotentialOwner.ToString().Equals(role));

            TaskEntity task = new TaskEntity
            {
                TaskId = new TaskId(),
                Status = TaskStatus.Suspended,
                SuspendedState = new SuspendedState { OriginalStatus = TaskStatus.InProgress },
                Initiator = initiator.Identity,
                ActualOwner = caller.Identity
            };
            TaskLifecycle lifecycle = new TaskLifecycle();
            lifecycle.Resume(task, caller);
        }



        [Test]
        public void TestComplete()
        {
            string role = string.Empty;
            IPrincipal caller = SetupMockPrincipal("Caller",
                new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.ActualOwner.ToString().Equals(role));

            TaskEntity task = new TaskEntity
            {
                TaskId = new TaskId(),
                Status = TaskStatus.InProgress,
                ActualOwner = caller.Identity
            };
            TaskLifecycle lifecycle = new TaskLifecycle();
            lifecycle.Complete(task, caller);
            Assert.AreEqual(TaskStatus.Completed,task.Status);
            Assert.IsNotNull(task.Completed);
        }

        [Test]
        [ExpectedException(typeof(TaskInvalidStateException))]
        public void TestCompleteInvalidState()
        {
            string role = string.Empty;
            IPrincipal caller = SetupMockPrincipal("Caller",
                new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.ActualOwner.ToString().Equals(role));

            TaskEntity task = new TaskEntity
            {
                TaskId = new TaskId(),
                Status = TaskStatus.Ready,
                ActualOwner = caller.Identity
            };
            TaskLifecycle lifecycle = new TaskLifecycle();
            lifecycle.Complete(task, caller);
        }
        [Test]
        [ExpectedException(typeof(TaskAccessException))]
        public void TestCompleteInvalidRole()
        {
            IPrincipal caller = SetupMockPrincipal("Caller",
                new IdentityId(), 
                s => {  },
                () => false);

            TaskEntity task = new TaskEntity
            {
                TaskId = new TaskId(),
                Status = TaskStatus.InProgress,
                ActualOwner = caller.Identity
            };
            TaskLifecycle lifecycle = new TaskLifecycle();
            lifecycle.Complete(task, caller);
        }

        [Test]
        public void TestFail()
        {
            string role = string.Empty;
            IPrincipal caller = SetupMockPrincipal("Caller",
                new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.ActualOwner.ToString().Equals(role));

            TaskEntity task = new TaskEntity
            {
                TaskId = new TaskId(),
                Status = TaskStatus.InProgress,
                ActualOwner = caller.Identity
            };
            TaskLifecycle lifecycle = new TaskLifecycle();
            lifecycle.Fail(task,new Fault(), caller);
            Assert.AreEqual(TaskStatus.Failed, task.Status);
        }

        [Test]
        [ExpectedException(typeof(TaskInvalidStateException))]
        public void TestFailInvalidState()
        {
            string role = string.Empty;
            IPrincipal caller = SetupMockPrincipal("Caller",
                new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.ActualOwner.ToString().Equals(role));

            TaskEntity task = new TaskEntity
            {
                TaskId = new TaskId(),
                Status = TaskStatus.Ready,
                ActualOwner = caller.Identity
            };
            TaskLifecycle lifecycle = new TaskLifecycle();
            lifecycle.Fail(task, new Fault(), caller);
        }

        [Test]
        [ExpectedException(typeof(TaskAccessException))]
        public void TestFailInvalidRole()
        {
            IPrincipal caller = SetupMockPrincipal("Caller",
                new IdentityId(), 
                s => { },
                () => false);

            TaskEntity task = new TaskEntity
            {
                TaskId = new TaskId(),
                Status = TaskStatus.InProgress,
                ActualOwner = caller.Identity
            };
            TaskLifecycle lifecycle = new TaskLifecycle();
            lifecycle.Fail(task, new Fault(), caller);
        }

        [Test]
        public void TestSkip()
        {
            string role = string.Empty;
            IPrincipal initiator = SetupMockPrincipal("Initiator",
                new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.Initiator.ToString().Equals(role));
            IPrincipal caller = SetupMockPrincipal("Caller",
                new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.ActualOwner.ToString().Equals(role));

            TaskEntity task = new TaskEntity
            {
                TaskId = new TaskId(),
                Status = TaskStatus.Ready,
                Initiator = initiator.Identity,
                ActualOwner = caller.Identity,
                IsSkippable = true
            };
            TaskLifecycle lifecycle = new TaskLifecycle();
            lifecycle.Skip(task, caller);
            Assert.AreEqual(TaskStatus.Completed,task.Status);
            Assert.IsNotNull(task.Completed);
        }

        [Test]
        [ExpectedException(typeof(TaskInvalidStateException))]
        public void TestSkipNotSkippable()
        {
            string role = string.Empty;
            IPrincipal initiator = SetupMockPrincipal("Initiator",
                new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.Initiator.ToString().Equals(role));
            IPrincipal caller = SetupMockPrincipal("Caller",
                new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.PotentialOwner.ToString().Equals(role));

            TaskEntity task = new TaskEntity
            {
                TaskId = new TaskId(),
                Status = TaskStatus.Ready,
                Initiator = initiator.Identity,
                ActualOwner = caller.Identity,
                IsSkippable = false
            };
            TaskLifecycle lifecycle = new TaskLifecycle();
            lifecycle.Skip(task, caller);
        }

        [Test]
        [ExpectedException(typeof(TaskAccessException))]
        public void TestSkipInvalidRole()
        {
            string role = string.Empty;
            IPrincipal initiator = SetupMockPrincipal("Initiator",new IdentityId(), 
                                                      s => { role = s; },
                                                      () => HumanRoles.Initiator.ToString().Equals(role));
            IPrincipal caller = SetupMockPrincipal("Caller", new IdentityId(), 
                                                   s => { role = s; },
                                                   () => false);

            TaskEntity task = new TaskEntity
                                  {
                                      TaskId = new TaskId(),
                                      Status = TaskStatus.Ready,
                                      Initiator = initiator.Identity,
                                      ActualOwner = caller.Identity,
                                      IsSkippable = true
                                  };
            TaskLifecycle lifecycle = new TaskLifecycle();
            lifecycle.Skip(task, caller);
        }


        public void TestForwardFromReady()
        {
            string role = string.Empty;
            IPrincipal initiator = SetupMockPrincipal("Initiator", new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.Initiator.ToString().Equals(role));
            IPrincipal caller = SetupMockPrincipal("Caller", new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.PotentialOwner.ToString().Equals(role));
            IPrincipal target = SetupMockPrincipal("Target", new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.ActualOwner.ToString().Equals(role));

            TaskEntity task = new TaskEntity
            {
                TaskId = new TaskId(),
                Status = TaskStatus.Ready,
                Initiator = initiator.Identity,
                ActualOwner = caller.Identity,
            };
            TaskLifecycle lifecycle = new TaskLifecycle();
            lifecycle.Forward(task, target.Identity, caller);
            Assert.AreEqual(TaskStatus.Ready, task.Status);
            //TODO: Check identity
        }
        [Test]
        public void TestForwardFromReserved()
        {
            string role = string.Empty;
            IPrincipal initiator = SetupMockPrincipal("Initiator", new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.Initiator.ToString().Equals(role));
            IPrincipal caller = SetupMockPrincipal("Caller", new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.ActualOwner.ToString().Equals(role));
            IPrincipal target = SetupMockPrincipal("Target", new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.ActualOwner.ToString().Equals(role));

            TaskEntity task = new TaskEntity
            {
                TaskId = new TaskId(),
                Status = TaskStatus.Reserved,
                Initiator = initiator.Identity,
                ActualOwner = caller.Identity,
            };
            TaskLifecycle lifecycle = new TaskLifecycle();
            lifecycle.Forward(task, target.Identity, caller);
            Assert.AreEqual(TaskStatus.Ready, task.Status);
            //TODO: Check identity
        }

        [Test]
        public void TestForwardFromInProgress()
        {
            string role = string.Empty;
            IPrincipal initiator = SetupMockPrincipal("Initiator", new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.Initiator.ToString().Equals(role));
            IPrincipal caller = SetupMockPrincipal("Caller", new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.ActualOwner.ToString().Equals(role));
            IPrincipal target = SetupMockPrincipal("Target", new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.ActualOwner.ToString().Equals(role));

            TaskEntity task = new TaskEntity
            {
                TaskId = new TaskId(),
                Status = TaskStatus.InProgress,
                Initiator = initiator.Identity,
                ActualOwner = caller.Identity,
            };
            TaskLifecycle lifecycle = new TaskLifecycle();
            lifecycle.Forward(task, target.Identity, caller);
            Assert.AreEqual(TaskStatus.Ready, task.Status);
            //TODO: Check identity
        }

        [Test]
        [ExpectedException(typeof(TaskInvalidStateException))]
        public void TestForwardFromUnsupportedState()
        {
            string role = string.Empty;
            IPrincipal initiator = SetupMockPrincipal("Initiator", new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.Initiator.ToString().Equals(role));
            IPrincipal caller = SetupMockPrincipal("Caller", new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.ActualOwner.ToString().Equals(role));
            IPrincipal target = SetupMockPrincipal("Target", new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.ActualOwner.ToString().Equals(role));

            TaskEntity task = new TaskEntity
            {
                TaskId = new TaskId(),
                Status = TaskStatus.Suspended,
                Initiator = initiator.Identity,
                ActualOwner = caller.Identity,
            };
            TaskLifecycle lifecycle = new TaskLifecycle();
            lifecycle.Forward(task, target.Identity, caller);
        }

        [Test]
        [ExpectedException(typeof(TaskAccessException))]
        public void TestForwardFromReadyInvalidRole()
        {
            string role = string.Empty;
            IPrincipal initiator = SetupMockPrincipal("Initiator", new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.Initiator.ToString().Equals(role));
            IPrincipal caller = SetupMockPrincipal("Caller", new IdentityId(), 
                s => { role = s; },
                () => false);
            IPrincipal target = SetupMockPrincipal("Target", new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.ActualOwner.ToString().Equals(role));

            TaskEntity task = new TaskEntity
            {
                TaskId = new TaskId(),
                Status = TaskStatus.Ready,
                Initiator = initiator.Identity,
                ActualOwner = caller.Identity,
            };
            TaskLifecycle lifecycle = new TaskLifecycle();
            lifecycle.Forward(task, target.Identity, caller);
        }

        [Test]
        [ExpectedException(typeof(TaskAccessException))]
        public void TestForwardFromReservedInvalidRole()
        {
            string role = string.Empty;
            IPrincipal initiator = SetupMockPrincipal("Initiator", new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.Initiator.ToString().Equals(role));
            IPrincipal caller = SetupMockPrincipal("Caller", new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.PotentialOwner.ToString().Equals(role));
            IPrincipal target = SetupMockPrincipal("Target", new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.ActualOwner.ToString().Equals(role));

            TaskEntity task = new TaskEntity
            {
                TaskId = new TaskId(),
                Status = TaskStatus.Reserved,
                Initiator = initiator.Identity,
                ActualOwner = caller.Identity,
            };
            TaskLifecycle lifecycle = new TaskLifecycle();
            lifecycle.Forward(task, target.Identity, caller);
        }

        [Test]
        [ExpectedException(typeof(TaskAccessException))]
        public void TestForwardFromInProgressInvalidRole()
        {
            string role = string.Empty;
            IPrincipal initiator = SetupMockPrincipal("Initiator", new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.Initiator.ToString().Equals(role));
            IPrincipal caller = SetupMockPrincipal("Caller", new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.PotentialOwner.ToString().Equals(role));
            IPrincipal target = SetupMockPrincipal("Target", new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.ActualOwner.ToString().Equals(role));

            TaskEntity task = new TaskEntity
            {
                TaskId = new TaskId(),
                Status = TaskStatus.InProgress,
                Initiator = initiator.Identity,
                ActualOwner = caller.Identity,
            };
            TaskLifecycle lifecycle = new TaskLifecycle();
            lifecycle.Forward(task, target.Identity, caller);
        }


        [Test]
        public void TestDelegate()
        {
            string role = string.Empty;
            IPrincipal initiator = SetupMockPrincipal("Initiator", new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.Initiator.ToString().Equals(role));
            IPrincipal caller = SetupMockPrincipal("Caller", new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.ActualOwner.ToString().Equals(role));
            IPrincipal target = SetupMockPrincipal("Target", new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.ActualOwner.ToString().Equals(role));

            TaskEntity task = new TaskEntity
            {
                TaskId = new TaskId(),
                Status = TaskStatus.InProgress,
                Initiator = initiator.Identity,
                ActualOwner = caller.Identity,
                Priority = Priority.Low
            };
            TaskLifecycle lifecycle = new TaskLifecycle();
            lifecycle.Delegate(task, target.Identity, Priority.High, caller);
            Assert.AreEqual(TaskStatus.Reserved,task.Status);
            Assert.AreEqual(Priority.High,task.Priority);
            //TODO: Check Actual Owner
        }
        [Test]
        [ExpectedException(typeof(TaskInvalidStateException))]
        public void TestDelegateInvalidState()
        {
            string role = string.Empty;
            IPrincipal initiator = SetupMockPrincipal("Initiator", new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.Initiator.ToString().Equals(role));
            IPrincipal caller = SetupMockPrincipal("Caller", new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.ActualOwner.ToString().Equals(role));
            IPrincipal target = SetupMockPrincipal("Target", new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.ActualOwner.ToString().Equals(role));

            TaskEntity task = new TaskEntity
            {
                TaskId = new TaskId(),
                Status = TaskStatus.Ready,
                Initiator = initiator.Identity,
                ActualOwner = caller.Identity,
                Priority = Priority.Low
            };
            TaskLifecycle lifecycle = new TaskLifecycle();
            lifecycle.Delegate(task, target.Identity, Priority.High, caller);
        }
        [Test]
        [ExpectedException(typeof(TaskAccessException))]
        public void TestDelegateInvalidRole()
        {
            string role = string.Empty;
            IPrincipal initiator = SetupMockPrincipal("Initiator", new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.Initiator.ToString().Equals(role));
            IPrincipal caller = SetupMockPrincipal("Caller", new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.PotentialOwner.ToString().Equals(role));
            IPrincipal target = SetupMockPrincipal("Target", new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.ActualOwner.ToString().Equals(role));

            TaskEntity task = new TaskEntity
            {
                TaskId = new TaskId(),
                Status = TaskStatus.InProgress,
                Initiator = initiator.Identity,
                ActualOwner = caller.Identity,
                Priority = Priority.Low
            };
            TaskLifecycle lifecycle = new TaskLifecycle();
            lifecycle.Delegate(task, target.Identity, Priority.High, caller);
        }

        [Test]
        public void TestActivateOnePotentialOwner()
        {
            string role = string.Empty;
            IPrincipal initiator = SetupMockPrincipal("Initiator", new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.Initiator.ToString().Equals(role));
            IPrincipal caller = SetupMockPrincipal("Caller", new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.BusinessAdministrator.ToString().Equals(role));
            IPrincipal target = SetupMockPrincipal("Target", new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.PotentialOwner.ToString().Equals(role));

            TaskEntity task = new TaskEntity
            {
                TaskId = new TaskId(),
                Status = TaskStatus.Created,
                Initiator = initiator.Identity,
            };
            TaskLifecycle lifecycle = new TaskLifecycle();
            IEnumerable<IIdentity> en = new List<IIdentity>{target.Identity};
            lifecycle.Activate(task,en,caller);
            Assert.IsNotNull(task.ActualOwner);
            Assert.AreEqual(TaskStatus.Reserved,task.Status);
        }

        [Test]
        public void TestActivateManyPotentialOwners()
        {
            string role = string.Empty;
            IPrincipal initiator = SetupMockPrincipal("Initiator", new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.Initiator.ToString().Equals(role));
            IPrincipal caller = SetupMockPrincipal("Caller", new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.BusinessAdministrator.ToString().Equals(role));
            IPrincipal target1 = SetupMockPrincipal("Target1", new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.PotentialOwner.ToString().Equals(role));
            IPrincipal target2 = SetupMockPrincipal("Target2", new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.PotentialOwner.ToString().Equals(role));

            TaskEntity task = new TaskEntity
            {
                TaskId = new TaskId(),
                Status = TaskStatus.Created,
                Initiator = initiator.Identity,
            };
            TaskLifecycle lifecycle = new TaskLifecycle();
            IEnumerable<IIdentity> en = new List<IIdentity> { target1.Identity, target2.Identity };
            lifecycle.Activate(task, en, caller);
            Assert.IsNull(task.ActualOwner);
            Assert.AreEqual(TaskStatus.Ready, task.Status);
        }

        [Test]
        [ExpectedException(typeof(TaskInvalidStateException))]
        public void TestActivateInvalidState()
        {
            string role = string.Empty;
            IPrincipal initiator = SetupMockPrincipal("Initiator", new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.Initiator.ToString().Equals(role));
            IPrincipal caller = SetupMockPrincipal("Caller", new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.BusinessAdministrator.ToString().Equals(role));
            IPrincipal target = SetupMockPrincipal("Target", new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.PotentialOwner.ToString().Equals(role));

            TaskEntity task = new TaskEntity
            {
                TaskId = new TaskId(),
                Status = TaskStatus.Ready,
                Initiator = initiator.Identity,
            };
            TaskLifecycle lifecycle = new TaskLifecycle();
            IEnumerable<IIdentity> en = new List<IIdentity> { target.Identity };
            lifecycle.Activate(task, en, caller);            
        }

        [Test]
        [ExpectedException(typeof(TaskAccessException))]
        public void TestActivateInvalidRole()
        {
            string role = string.Empty;
            IPrincipal initiator = SetupMockPrincipal("Initiator", new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.Initiator.ToString().Equals(role));
            IPrincipal caller = SetupMockPrincipal("Caller", new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.PotentialOwner.ToString().Equals(role));
            IPrincipal target = SetupMockPrincipal("Target", new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.PotentialOwner.ToString().Equals(role));

            TaskEntity task = new TaskEntity
            {
                TaskId = new TaskId(),
                Status = TaskStatus.Created,
                Initiator = initiator.Identity,
            };
            TaskLifecycle lifecycle = new TaskLifecycle();
            IEnumerable<IIdentity> en = new List<IIdentity> { target.Identity };
            lifecycle.Activate(task, en, caller);            
        }

        [Test]
        public void TestNominateUser()
        {
            string role = string.Empty;
            IPrincipal initiator = SetupMockPrincipal("Initiator", new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.Initiator.ToString().Equals(role));
            IPrincipal caller = SetupMockPrincipal("Caller", new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.BusinessAdministrator.ToString().Equals(role));
            IPrincipal target = SetupMockPrincipal("Target", new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.PotentialOwner.ToString().Equals(role));

            TaskEntity task = new TaskEntity
            {
                TaskId = new TaskId(),
                Status = TaskStatus.Created,
                Initiator = initiator.Identity,
            };
            TaskLifecycle lifecycle = new TaskLifecycle();
            lifecycle.Nominate(task, target.Identity, caller);
            Assert.IsNotNull(task.ActualOwner);
            Assert.AreEqual(TaskStatus.Reserved,task.Status);
        }

        [Test]
        public void TestNominateGroup()
        {
            string role = string.Empty;
            IPrincipal initiator = SetupMockPrincipal("Initiator", new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.Initiator.ToString().Equals(role));
            IPrincipal caller = SetupMockPrincipal("Caller", new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.BusinessAdministrator.ToString().Equals(role));
            IPrincipal target = SetupMockGroupPrincipal("Target", 
                new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.PotentialOwner.ToString().Equals(role));

            TaskEntity task = new TaskEntity
            {
                TaskId = new TaskId(),
                Status = TaskStatus.Created,
                Initiator = initiator.Identity,
            };
            TaskLifecycle lifecycle = new TaskLifecycle();
            lifecycle.Nominate(task, target.Identity, caller);
            Assert.IsNull(task.ActualOwner);
            Assert.AreEqual(TaskStatus.Ready, task.Status);
        }
        [Test]
        [ExpectedException(typeof(TaskInvalidStateException))]
        public void TestNominateInvalidState()
        {
            string role = string.Empty;
            IPrincipal initiator = SetupMockPrincipal("Initiator", new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.Initiator.ToString().Equals(role));
            IPrincipal caller = SetupMockPrincipal("Caller", new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.BusinessAdministrator.ToString().Equals(role));
            IPrincipal target = SetupMockPrincipal("Target", new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.PotentialOwner.ToString().Equals(role));

            TaskEntity task = new TaskEntity
            {
                TaskId = new TaskId(),
                Status = TaskStatus.Ready,
                Initiator = initiator.Identity,
            };
            TaskLifecycle lifecycle = new TaskLifecycle();
            lifecycle.Nominate(task, target.Identity, caller);
        }
        [Test]
        [ExpectedException(typeof(TaskAccessException))]
        public void TestNominateInvalidRole()
        {
            string role = string.Empty;
            IPrincipal initiator = SetupMockPrincipal("Initiator", new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.Initiator.ToString().Equals(role));
            IPrincipal caller = SetupMockPrincipal("Caller", new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.PotentialOwner.ToString().Equals(role));
            IPrincipal target = SetupMockPrincipal("Target", new IdentityId(), 
                s => { role = s; },
                () => HumanRoles.PotentialOwner.ToString().Equals(role));

            TaskEntity task = new TaskEntity
            {
                TaskId = new TaskId(),
                Status = TaskStatus.Created,
                Initiator = initiator.Identity,
            };
            TaskLifecycle lifecycle = new TaskLifecycle();
            lifecycle.Nominate(task, target.Identity, caller);
        }

        #endregion
        
    }
}
