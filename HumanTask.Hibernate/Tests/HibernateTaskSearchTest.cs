using System;
using System.Linq;
using HumanTask.Hibernate.Linq;
using HumanTask.Linq;
using KlaudWerk.Security;
using NUnit.Framework;

namespace HumanTask.Hibernate.Tests
{
    [TestFixture]
    public class HibernateTaskSearchTest : HibernateTaskTestBase
    {
        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            OnFixtureSetUp();
        }

        [SetUp]
        public void SetUp()
        {
            OnSetUp();
        }
        [TearDown]
        public void TestTearDown()
        {
            OnTestTearDown();
        }

        [Test]
        public void TestSearchByTaskName()
        {
            TaskEntity entity1 = new TaskEntity
            {
                TaskId = new TaskId(),
                Initiator = new IdentityId().GetIdentity(),
                Priority = Priority.Normal,
                Status = TaskStatus.Created,
                Created = DateTime.UtcNow,
                Name = "Name-1",
                Subject = "Subject"
            };
            TaskEntity entity2 = new TaskEntity
            {
                TaskId = new TaskId(),
                Initiator = new IdentityId().GetIdentity(),
                Priority = Priority.Normal,
                Status = TaskStatus.Created,
                Created = DateTime.UtcNow,
                Name = "Name-Two",
                Subject = "Subject"
            };
            TaskEntity entity3 = new TaskEntity
            {
                TaskId = new TaskId(),
                Initiator = new IdentityId().GetIdentity(),
                Priority = Priority.Normal,
                Status = TaskStatus.Created,
                Created = DateTime.UtcNow,
                Name = "Name-Three-3",
                Subject = "Subject"
            };

            TaskDao dao = new TaskDao(SessionFactory);
            dao.Store(entity1);
            dao.Store(entity2);
            dao.Store(entity3);
            QuerableTaskService tasks = new QuerableTaskService(
                new HibernateTaskQueryContext(SessionFactory));

            var t = from task in tasks where task.Name == "Name-1" select task;
            foreach (Task task in t)
            {
                Assert.IsNotNull(task);
            }
            Assert.IsNotNull(t);
            Assert.AreEqual(1, t.Count());
            Task foundTask = t.ElementAt(0);
            Assert.AreEqual("Name-1", foundTask.Name);
            Assert.AreEqual(entity1.TaskId, foundTask.Id);

        }

        [Test]
        public void TestSearchByTaskNameStartsWith()
        {
            TaskEntity entity1 = new TaskEntity
            {
                TaskId = new TaskId(),
                Initiator = new IdentityId().GetIdentity(),
                Priority = Priority.Normal,
                Status = TaskStatus.Created,
                Created = DateTime.UtcNow,
                Name = "Name-1",
                Subject = "Subject"
            };
            TaskEntity entity2 = new TaskEntity
            {
                TaskId = new TaskId(),
                Initiator = new IdentityId().GetIdentity(),
                Priority = Priority.Normal,
                Status = TaskStatus.Created,
                Created = DateTime.UtcNow,
                Name = "Name-Two",
                Subject = "Subject"
            };
            TaskEntity entity3 = new TaskEntity
            {
                TaskId = new TaskId(),
                Initiator = new IdentityId().GetIdentity(),
                Priority = Priority.Normal,
                Status = TaskStatus.Created,
                Created = DateTime.UtcNow,
                Name = "Three-Name-3",
                Subject = "Subject"
            };

            TaskDao dao = new TaskDao(SessionFactory);
            dao.Store(entity1);
            dao.Store(entity2);
            dao.Store(entity3);
            QuerableTaskService tasks = new QuerableTaskService(
                new HibernateTaskQueryContext(SessionFactory));

            var t = from task in tasks where task.Name.StartsWith("Name") select task;
            foreach (Task task in t)
            {
                Assert.IsNotNull(task);
            }
            Assert.IsNotNull(t);
            Assert.AreEqual(2, t.Count());
            Task foundTask = t.ElementAt(0);
            Assert.IsTrue(foundTask.Name.StartsWith("Name"));
            foundTask = t.ElementAt(1);
            Assert.IsTrue(foundTask.Name.StartsWith("Name"));
            Assert.IsTrue(t.ElementAt(0).Id != t.ElementAt(1).Id);

        }

        [Test]
        public void TestSearchByTaskNameEndsWith()
        {
            TaskEntity entity1 = new TaskEntity
            {
                TaskId = new TaskId(),
                Initiator = new IdentityId().GetIdentity(),
                Priority = Priority.Normal,
                Status = TaskStatus.Created,
                Created = DateTime.UtcNow,
                Name = "Name-1-Task",
                Subject = "1"
            };
            TaskEntity entity2 = new TaskEntity
            {
                TaskId = new TaskId(),
                Initiator = new IdentityId().GetIdentity(),
                Priority = Priority.Normal,
                Status = TaskStatus.Created,
                Created = DateTime.UtcNow,
                Name = "Name-Two",
                Subject = "2"
            };
            TaskEntity entity3 = new TaskEntity
            {
                TaskId = new TaskId(),
                Initiator = new IdentityId().GetIdentity(),
                Priority = Priority.Normal,
                Status = TaskStatus.Created,
                Created = DateTime.UtcNow,
                Name = "Name-Three-3-Task",
                Subject = "3"
            };

            TaskDao dao = new TaskDao(SessionFactory);
            dao.Store(entity1);
            dao.Store(entity2);
            dao.Store(entity3);
            QuerableTaskService tasks = new QuerableTaskService(
                new HibernateTaskQueryContext(SessionFactory));

            var t = from task in tasks where task.Name.EndsWith("Task") select task;
            foreach (Task task in t)
            {
                Assert.IsNotNull(task);
            }
            Assert.IsNotNull(t);
            Assert.AreEqual(2, t.Count());
            Task foundTask = t.ElementAt(0);
            Assert.IsTrue(foundTask.Name.EndsWith("Task"));
            foundTask = t.ElementAt(1);
            Assert.IsTrue(foundTask.Name.EndsWith("Task"));
            Assert.IsTrue(t.ElementAt(0).Id != t.ElementAt(1).Id);

        }

        [Test]
        public void TestSearchByTaskNameContains()
        {
            TaskEntity entity1 = new TaskEntity
            {
                TaskId = new TaskId(),
                Initiator = new IdentityId().GetIdentity(),
                Priority = Priority.Normal,
                Status = TaskStatus.Created,
                Created = DateTime.UtcNow,
                Name = "Name-One-1",
                Subject = "Subject"
            };
            TaskEntity entity2 = new TaskEntity
            {
                TaskId = new TaskId(),
                Initiator = new IdentityId().GetIdentity(),
                Priority = Priority.Normal,
                Status = TaskStatus.Created,
                Created = DateTime.UtcNow,
                Name = "Name-One-Two",
                Subject = "Subject"
            };
            TaskEntity entity3 = new TaskEntity
            {
                TaskId = new TaskId(),
                Initiator = new IdentityId().GetIdentity(),
                Priority = Priority.Normal,
                Status = TaskStatus.Created,
                Created = DateTime.UtcNow,
                Name = "Name-Three-3",
                Subject = "Subject"
            };

            TaskDao dao = new TaskDao(SessionFactory);
            dao.Store(entity1);
            dao.Store(entity2);
            dao.Store(entity3);
            QuerableTaskService tasks = new QuerableTaskService(
                new HibernateTaskQueryContext(SessionFactory));

            var t = from task in tasks where task.Name.Contains("One") select task;
            foreach (Task task in t)
            {
                Assert.IsNotNull(task);
            }
            Assert.IsNotNull(t);
            Assert.AreEqual(2, t.Count());
            Task foundTask = t.ElementAt(0);
            Assert.IsTrue(foundTask.Name.Contains("One"));
            foundTask = t.ElementAt(1);
            Assert.IsTrue(foundTask.Name.Contains("One"));
            Assert.IsTrue(t.ElementAt(0).Id!=t.ElementAt(1).Id);
        }

        [Test]
        public void TestSearchSimpleAnd()
        {
            TaskEntity entity1 = new TaskEntity
            {
                TaskId = new TaskId(),
                Initiator = new IdentityId().GetIdentity(),
                Priority = Priority.Low,
                Status = TaskStatus.Created,
                Created = DateTime.UtcNow,
                Name = "Name-1",
                Subject = "Subject"
            };
            TaskEntity entity2 = new TaskEntity
            {
                TaskId = new TaskId(),
                Initiator = new IdentityId().GetIdentity(),
                Priority = Priority.High,
                Status = TaskStatus.Created,
                Created = DateTime.UtcNow,
                Name = "Name-Two",
                Subject = "Subject"
            };
            TaskEntity entity3 = new TaskEntity
            {
                TaskId = new TaskId(),
                Initiator = new IdentityId().GetIdentity(),
                Priority = Priority.Normal,
                Status = TaskStatus.Created,
                Created = DateTime.UtcNow,
                Name = "Name-Three-3",
                Subject = "Subject"
            };

            TaskDao dao = new TaskDao(SessionFactory);
            dao.Store(entity1);
            dao.Store(entity2);
            dao.Store(entity3);
            QuerableTaskService tasks = new QuerableTaskService(
                new HibernateTaskQueryContext(SessionFactory));

            var t= from task in tasks where task.Name.StartsWith("Name") 
                       && task.Priority>Priority.Normal select task;

            Assert.IsNotNull(t);
            foreach (Task task in t)
            {
                Assert.IsNotNull(task);
            }
            Assert.AreEqual(1, t.Count());
            Task foundTask = t.ElementAt(0);
            Assert.AreEqual("Name-Two", foundTask.Name);
            Assert.AreEqual(entity2.TaskId, foundTask.Id);

        }

        [Test]
        public void TestSearchSimpleAndOr()
        {
            TaskEntity entity1 = new TaskEntity
            {
                TaskId = new TaskId(),
                Initiator = new IdentityId().GetIdentity(),
                Priority = Priority.Normal,
                Status = TaskStatus.Failed,
                Created = DateTime.UtcNow,
                Name = "Name-1",
                Subject = "Subject"
            };
            TaskEntity entity2 = new TaskEntity
            {
                TaskId = new TaskId(),
                Initiator = new IdentityId().GetIdentity(),
                Priority = Priority.High,
                Status = TaskStatus.Ready,
                Created = DateTime.UtcNow,
                Name = "Name-Two",
                Subject = "Subject"
            };
            TaskEntity entity3 = new TaskEntity
            {
                TaskId = new TaskId(),
                Initiator = new IdentityId().GetIdentity(),
                Priority = Priority.Normal,
                Status = TaskStatus.InProgress,
                Created = DateTime.UtcNow,
                Name = "Name-Three-3",
                Subject = "Subject"
            };

            TaskDao dao = new TaskDao(SessionFactory);
            dao.Store(entity1);
            dao.Store(entity2);
            dao.Store(entity3);
            QuerableTaskService tasks = new QuerableTaskService(
                new HibernateTaskQueryContext(SessionFactory));

            var t = from task in tasks
                    where task.Name.StartsWith("Name")
                        && task.Priority > Priority.Normal
                        || task.Status==TaskStatus.InProgress
                    select task;

            Assert.IsNotNull(t);
            foreach (Task task in t)
            {
                Assert.IsNotNull(task);
            }
            Assert.AreEqual(2, t.Count());
            Task foundTask = t.ElementAt(0);
            Assert.AreEqual("Name-Two", foundTask.Name);
            Assert.AreEqual(entity2.TaskId, foundTask.Id);
            foundTask = t.ElementAt(1);
            Assert.AreEqual("Name-Three-3", foundTask.Name);
            Assert.AreEqual(entity3.TaskId, foundTask.Id);

        }

        [Test]
        public void TestSearchSimpleOr()
        {
            throw new NotImplementedException();
        }
        [Test]
        public void TestSearchLike()
        {
            throw new NotImplementedException();
        }
        [Test]
        public void TestSearchIn()
        {
            throw new NotImplementedException();
        }
        [Test]
        public void TestSearchBetween()
        {
            throw new NotImplementedException();
        }
        [Test]
        public void TestSearchCollectionNotContains()
        {
            throw new NotImplementedException();
        }
        [Test]
        public void TestSearchCollectionContains()
        {
            Guid userGuid1 = Guid.NewGuid();
            Guid userGuid2 = Guid.NewGuid();
            TaskEntity entity1 = new TaskEntity
            {
                TaskId = new TaskId(),
                Initiator = new IdentityId().GetIdentity(),
                Priority = Priority.Normal,
                Status = TaskStatus.Failed,
                Created = DateTime.UtcNow,
                Name = "Name-1",
                Subject = "Subject"
            };
            TaskEntity entity2 = new TaskEntity
            {
                TaskId = new TaskId(),
                Initiator = new IdentityId().GetIdentity(),
                Priority = Priority.High,
                Status = TaskStatus.Ready,
                Created = DateTime.UtcNow,
                Name = "Name-Two",
                Subject = "Subject"
            };
            TaskEntity entity3 = new TaskEntity
            {
                TaskId = new TaskId(),
                Initiator = new IdentityId().GetIdentity(),
                Priority = Priority.Normal,
                Status = TaskStatus.InProgress,
                Created = DateTime.UtcNow,
                Name = "Name-Three-3",
                Subject = "Subject"
            };
            entity2.PotentialOwners.Add(new IdentityId(userGuid1).GetIdentity());
            entity2.PotentialOwners.Add(new IdentityId(userGuid2).GetIdentity());
            entity1.BusinessAdministrators.Add(new IdentityId(userGuid1).GetIdentity());
            TaskDao dao = new TaskDao(SessionFactory);
            dao.Store(entity1);
            dao.Store(entity2);
            dao.Store(entity3);
            QuerableTaskService tasks = new QuerableTaskService(
                new HibernateTaskQueryContext(SessionFactory));
            var t = from task in tasks
                    where task.PotentialOwners.Contains(new IdentityId(userGuid1).GetIdentity())
                    select task;
            Assert.IsNotNull(t);
            foreach(var task in t)
            {
                Assert.IsNotNull(task);
            }
            Assert.AreEqual(1,t.Count());
            Task foundTask = t.ElementAt(0);
            Assert.IsNotNull(foundTask);
            Assert.AreEqual(entity2.TaskId,foundTask.Id);
        }
        [Test]
        public void TestSearchComplexAnd()
        {
            throw new NotImplementedException();
        }
        [Test]
        public void TestSearchComplexOr()
        {
            throw new NotImplementedException();
        }
        [Test]
        public void TestSearchOrAndCombinedExpression()
        {
            throw new NotImplementedException();
        }
    }
}
