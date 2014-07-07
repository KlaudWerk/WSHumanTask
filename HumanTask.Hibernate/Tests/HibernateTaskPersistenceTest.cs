using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using KlaudWerk.Security;
using Moq;
using NHibernate;
using NHibernate.Linq;
using NUnit.Framework;

namespace HumanTask.Hibernate.Tests
{
    [TestFixture]
    public class HibernateTaskPersistenceTest:HibernateTaskTestBase
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
        public void TestStoreTaskEntity()
        {
            TaskEntity entity=new TaskEntity
                                  {
                                      TaskId = new TaskId(),
                                      Initiator = new IdentityId().GetIdentity(),
                                      Priority = Priority.Normal,
                                      Status = TaskStatus.Created,
                                      Created = DateTime.UtcNow,
                                      Name="Name",
                                      Subject="Subject"
                                  };
            ISession session = SessionFactory.OpenSession();
            session.Save(entity);
            session.Flush();
        }
        [Test]
        public void TestStoreTaskComments()
        {
            TaskEntity entity = new TaskEntity
            {
                TaskId = new TaskId(),
                Initiator = new IdentityId().GetIdentity(),
                Priority = Priority.Normal,
                Status = TaskStatus.Created,
                Created = DateTime.UtcNow,
                Name = "Name",
                Subject = "Subject",
                Comments = new List<TaskComment>()
            };
            entity.Comments.Add(new TaskComment
                                    {
                                        TimeStamp = DateTime.UtcNow,
                                        Comment = "comment 1",
                                        UserId = new IdentityId()
                                    });
            entity.Comments.Add(new TaskComment
            {
                TimeStamp = DateTime.UtcNow,
                Comment = "comment 2",
                UserId = new IdentityId()
            });

            entity.Comments.Add(new TaskComment
            {
                TimeStamp = DateTime.UtcNow,
                Comment = "comment 3",
                UserId = new IdentityId()
            });
            ISession session = SessionFactory.OpenSession();
            session.Save(entity);
            session.Flush();
            TaskEntity te  = session.Get<TaskEntity>(1);
            Assert.IsNotNull(te);
        }
        [Test]
        public void TestStoreFetchSubtasks()
        {
            TaskEntity entity = new TaskEntity
            {
                TaskId = new TaskId(),
                Initiator = new IdentityId().GetIdentity(),
                Priority = Priority.Normal,
                Status = TaskStatus.Created,
                Created = DateTime.UtcNow,
                Name = "Name",
                Subject = "Subject",
            };
            int subtaskId;
            using (ISession session = SessionFactory.OpenSession())
            {
                int parentId = Convert.ToInt32(session.Save(entity));
                session.Flush();
                TaskEntity te = session.Get<TaskEntity>(parentId);
                TaskEntity subtask = new TaskEntity
                                         {
                                             TaskId = new TaskId(),
                                             Initiator = new IdentityId().GetIdentity(),
                                             Priority = Priority.Normal,
                                             Status = TaskStatus.Created,
                                             Created = DateTime.UtcNow,
                                             Name = "Child Name",
                                             Subject = "Child Subject",
                                             Comments = new List<TaskComment>(),
                                             Parent = te
                                         };
                subtaskId = Convert.ToInt32(session.Save(subtask));
                session.Flush();
            }
            using (ISession session = SessionFactory.OpenSession())
            {
                TaskEntity teSubtask = session.Get<TaskEntity>(subtaskId);
                Assert.IsNotNull(teSubtask.Parent);
                Assert.AreEqual(entity.TaskId, teSubtask.Parent.TaskId);
                TaskEntity teParent = session.Load<TaskEntity>(((TaskEntity)teSubtask.Parent).Id);
                Assert.IsNotNull(teParent);
                Assert.IsNotNull(teParent.Subtasks);
                Assert.AreEqual(1, teParent.Subtasks.Count());
            }
        }

        [Test]
        public void TestTaskEntityQuery()
        {
            TaskEntity entity = new TaskEntity
            {
                TaskId = new TaskId(),
                Initiator = new IdentityId().GetIdentity(),
                Priority = Priority.Normal,
                Status = TaskStatus.Created,
                Created = DateTime.UtcNow,
                Name = "Name",
                Subject = "Subject",
            };
            using (ISession session = SessionFactory.OpenSession())
            {
                session.Save(entity);
                session.Flush();
            }
            using (ISession session = SessionFactory.OpenSession())
            {
                TaskEntity te =session.Query<TaskEntity>().Where(t => t.TaskId == entity.TaskId).FirstOrDefault();
                Assert.IsNotNull(te);
            }
            
        }

        [Test]
        public void TestCreateUpdateTask()
        {
            TaskEntity entity = new TaskEntity
            {
                TaskId = new TaskId(),
                Initiator = new IdentityId().GetIdentity(),
                Priority = Priority.Normal,
                Status = TaskStatus.Created,
                Created = DateTime.UtcNow,
                Name = "Name",
                Subject = "Subject"
            };

            Task task = new Task(entity)
                            {
                                LoggingService = new Mock<ILoggingService>().Object,
                                Priority = Priority.High

                            };
           
            TaskDao dao=new TaskDao(SessionFactory);
            task.Accept(dao);
            //update priority
            task.Priority = Priority.Medium;
            task.Name = "New Name";
            task.Subject = "New Subject";
            task.Accept(dao);
            task.AddComment("Bad comment");
            task.AddComment("Good comment");
            task.Accept(dao);
            TrickVisitor visitor=new TrickVisitor
                                     {
                                         OnEntityVisit = e=>e.Version=1
                                     };
            task.Accept(visitor);
            task.Name = "123";
            task.Accept(dao);


        }
        [Test]
        public void TestPersistRoleAssignment()
        {
            TaskEntity entity = new TaskEntity
            {
                TaskId = new TaskId(),
                Initiator = new IdentityId().GetIdentity(),
                Priority = Priority.Normal,
                Status = TaskStatus.Created,
                Created = DateTime.UtcNow,
                Name = "Name",
                Subject = "Subject"
            };

            Task task = new Task(entity)
            {
                LoggingService = new Mock<ILoggingService>().Object,
                Priority = Priority.High

            };

            TaskDao dao = new TaskDao(SessionFactory);
            task.Accept(dao);
            task.PotentialOwners.Add(new IdentityId().GetIdentity());
            task.PotentialOwners.Add(new IdentityId().GetIdentity());
            task.Recepients.Add(new IdentityId().GetIdentity());
            task.Recepients.Add(new IdentityId().GetIdentity());
            task.Recepients.Add(new IdentityId().GetIdentity());
            task.Recepients.Add(new IdentityId().GetIdentity());
            task.BusinessAdministrators.Add(new IdentityId().GetIdentity());
            task.Accept(dao);

            using (ISession session = SessionFactory.OpenSession())
            {
                TaskEntity te = session.Query<TaskEntity>().Where(t => t.TaskId == entity.TaskId).FirstOrDefault();
            }
        }
        [Test]
        public void TestTaskReady()
        {
            TaskEntity entity = new TaskEntity
            {
                TaskId = new TaskId(),
                Initiator = new IdentityId().GetIdentity(),
                Priority = Priority.Normal,
                Status = TaskStatus.Created,
                Created = DateTime.UtcNow,
                Name = "Name",
                Subject = "Subject"
            };

            Task task = new Task(entity)
            {
                LoggingService = new Mock<ILoggingService>().Object,
                Priority = Priority.High

            };

            TaskDao dao = new TaskDao(SessionFactory);
            task.Accept(dao);
            Task loaded;
            using (ISession session = SessionFactory.OpenSession())
            {
                TaskEntity te = session.Query<TaskEntity>().Where(t => t.TaskId == entity.TaskId).FirstOrDefault();
                loaded=new Task(te);
            }
            Thread.GetDomain().SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);
            IPrincipal p = Thread.CurrentPrincipal;
            loaded.Start();
            task.Accept(dao);
            loaded.Claim();
            task.Accept(dao);

        }
        private class TrickVisitor:ITaskVisitor
        {
            #region Implementation of ITaskVisitor

            public Action<Task> OnTaskVisit { get; set; }
            public Action<TaskEntity> OnEntityVisit { get; set; }
            /// <summary>
            /// Visits the specified task.
            /// </summary>
            /// <param name="task">The task.</param>
            public void Visit(Task task)
            {
                if(OnTaskVisit!=null)
                    OnTaskVisit.Invoke(task);
            }

            /// <summary>
            /// Visits the specified entity.
            /// </summary>
            /// <param name="entity">The entity.</param>
            public void Visit(TaskEntity entity)
            {
                if(OnEntityVisit!=null)
                    OnEntityVisit.Invoke(entity);
            }

            #endregion
        }
    }
}
