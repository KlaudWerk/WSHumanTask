using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using HumanTask.Linq;
using KlaudWerk.Security;
using Moq;
using Moq.Protected;
using NUnit.Framework;

namespace HumanTask.Test
{
    [TestFixture]
    public class SearchTest
    {
        [Test]
        public void TestEnsureSimpePropertySearch()
        {
            Mock<TaskExpressionTranslatorBase> mock = new Mock<TaskExpressionTranslatorBase>
                                                          {
                                                              CallBase = true
                                                          };
            IIdentity initiatorIdentity = new IdentityId(new Guid("56FDB6D4-D3AA-4C97-964A-6E8660A92FBC")).GetIdentity();
            DateTime now = DateTime.UtcNow;
            mock.Protected().Setup("QueryStatus", 
                ItExpr.Is<Operation>(o=>o==Operation.Equals), 
                ItExpr.Is<object>(v => (TaskStatus)v == TaskStatus.Ready)).Verifiable();
            mock.Protected().Setup("QueryPriority", 
                ItExpr.Is<Operation>(o=>o==Operation.Equals), 
                ItExpr.Is<object>(v => (Priority)v == Priority.Normal)).Verifiable();
            mock.Protected().Setup("QueryId", 
                ItExpr.Is<Operation>(o=>o==Operation.Equals), 
                ItExpr.Is<object>(v => (TaskId) v ==new TaskId(new Guid("56FDB6D4-D3AA-4C97-964A-6E8660A92FBC")))).Verifiable();
            mock.Protected().Setup("QueryCreated", 
                ItExpr.Is<Operation>(o=>o==Operation.LessOrEquals),
                ItExpr.Is<object>(v => ((DateTime)v).Equals(now - new TimeSpan(20, 0, 0)))).Verifiable();
            mock.Protected().Setup("QueryStarted", 
                ItExpr.Is<Operation>(o=>o==Operation.GreaterOrEquals), 
                ItExpr.Is<object>(v => ((DateTime)v).Equals(now-new TimeSpan(10,0,0)))).Verifiable();
            mock.Protected().Setup("QueryCompleted", 
                ItExpr.Is<Operation>(o=>o==Operation.Less), 
                ItExpr.Is<object>(v =>((DateTime)v).Equals(now))).Verifiable();
            mock.Protected().Setup("QueryTaskOutcome", 
                ItExpr.Is<Operation>(o=>o==Operation.Equals), 
                ItExpr.Is<object>(v => (string)v == "success")).Verifiable();
            mock.Protected().Setup("QueryInitiator", 
                ItExpr.Is<Operation>(o=>o==Operation.Equals),
                ItExpr.Is<object>(v => ((IIdentity)v).GetMappedId().Id == new Guid("56FDB6D4-D3AA-4C97-964A-6E8660A92FBC"))).Verifiable();
            mock.Protected().Setup("QueryActualOwner", 
                ItExpr.Is<Operation>(o=>o==Operation.NotEqual), 
                ItExpr.Is<object>(v => v == null)).Verifiable();
            mock.Setup(m => m.Execute()).Returns(new List<Task>());

            Mock<TaskQueryContext> ctxMock=new Mock<TaskQueryContext>{CallBase = true};
            ctxMock.Protected().Setup<TaskExpressionTranslatorBase>("InstantiateTranslator").Returns(mock.Object);
            
            QuerableTaskService tasks = new QuerableTaskService(ctxMock.Object);

            var t = from task in tasks
                    where
                        task.Priority == GetPriority()
                        &&
                        task.Status == TaskStatus.Ready
                        &&
                        task.Id == new TaskId(new Guid("56FDB6D4-D3AA-4C97-964A-6E8660A92FBC")) 
                        &&
                        task.Created<=now-new TimeSpan(20,0,0)
                        &&
                        task.Started>=now-new TimeSpan(10,0,0)
                        &&
                        task.Completed<now
                        &&
                        task.TaskOutcome=="success"
                        &&
                        task.Initiator == initiatorIdentity
                        &&
                        task.ActualOwner!=null
                    select task;
            foreach (Task task in t)
            {
                //execute the query
            }
            // verify executions
            mock.Protected().Verify("QueryStatus", Times.Once(),
                ItExpr.Is<Operation>(o => o == Operation.Equals),
                ItExpr.Is<object>(v => (TaskStatus)v == TaskStatus.Ready));
            mock.Protected().Verify("QueryPriority", Times.Once(),
                ItExpr.Is<Operation>(o => o == Operation.Equals),
                ItExpr.Is<object>(v => (Priority)v == Priority.Normal));
            mock.Protected().Verify("QueryId", Times.Once(),
                ItExpr.Is<Operation>(o => o == Operation.Equals),
                ItExpr.Is<object>(v => (TaskId)v == new TaskId(new Guid("56FDB6D4-D3AA-4C97-964A-6E8660A92FBC"))));
            mock.Protected().Verify("QueryCreated", Times.Once(),
                ItExpr.Is<Operation>(o => o == Operation.LessOrEquals),
                ItExpr.Is<object>(v => ((DateTime)v).Equals(now - new TimeSpan(20, 0, 0))));
            mock.Protected().Verify("QueryStarted", Times.Once(),
                ItExpr.Is<Operation>(o => o == Operation.GreaterOrEquals),
                ItExpr.Is<object>(v => ((DateTime)v).Equals(now - new TimeSpan(10, 0, 0))));
            mock.Protected().Verify("QueryCompleted", Times.Once(),
                ItExpr.Is<Operation>(o => o == Operation.Less),
                ItExpr.Is<object>(v => ((DateTime)v).Equals(now)));
            mock.Protected().Verify("QueryTaskOutcome", Times.Once(),
                ItExpr.Is<Operation>(o => o == Operation.Equals),
                ItExpr.Is<object>(v => (string)v == "success"));
            mock.Protected().Verify("QueryInitiator", Times.Once(),
                ItExpr.Is<Operation>(o => o == Operation.Equals),
                ItExpr.Is<object>(v => ((IIdentity)v).GetMappedId().Id == new Guid("56FDB6D4-D3AA-4C97-964A-6E8660A92FBC")));
            mock.Protected().Verify("QueryActualOwner", Times.Once(),
                ItExpr.Is<Operation>(o => o == Operation.NotEqual),
                ItExpr.Is<object>(v => v == null));

        }

        [Test]
        public void TestEnsureSimpleStringPropertySearch()
        {
            Mock<TaskExpressionTranslatorBase> mock = new Mock<TaskExpressionTranslatorBase>
            {
                CallBase = true
            };
            mock.Setup(m => m.Execute()).Returns(new List<Task>());
            mock.Protected().Setup("QueryName",
                    ItExpr.Is<Operation>(o => o == Operation.Equals),
                    ItExpr.Is<object>(v =>(string)v == "Test")).Verifiable();
            mock.Protected().Setup("QuerySubject",
                    ItExpr.Is<Operation>(o => o == Operation.Equals),
                    ItExpr.Is<object>(v => (string)v == "Subject")).Verifiable();

            Mock<TaskQueryContext> ctxMock = new Mock<TaskQueryContext> { CallBase = true };
            ctxMock.Protected().Setup<TaskExpressionTranslatorBase>("InstantiateTranslator").Returns(mock.Object);
            QuerableTaskService tasks = new QuerableTaskService(ctxMock.Object);
            var t = from task in tasks
                    where
                        task.Name=="Test"
                        &&
                        task.Subject=="Subject"
                    select task;
            foreach (Task task in t)
            {
                //execute the query
            }
            mock.Protected().Verify("QueryName",Times.Once(),
                ItExpr.Is<Operation>(o => o == Operation.Equals),
                ItExpr.Is<object>(v => (string)v == "Test"));
            mock.Protected().Verify("QuerySubject", Times.Once(),
                ItExpr.Is<Operation>(o => o == Operation.Equals),
                ItExpr.Is<object>(v => (string)v == "Subject"));
        }


        [Test]
        public void TestEnsureAssignmentsPropertySearch()
        {
            Mock<TaskExpressionTranslatorBase> mock = new Mock<TaskExpressionTranslatorBase>
            {
                CallBase = true
            };
            mock.Setup(m => m.Execute()).Returns(new List<Task>());
            mock.Protected().Setup("QueryPotentialOwners",
                    ItExpr.Is<Operation>(o => o == Operation.Contains),
                    ItExpr.IsAny<object>()).Verifiable();
            mock.Protected().Setup("QueryActualOwner",
                    ItExpr.Is<Operation>(o => o == Operation.Equals),
                    ItExpr.IsAny<object>()).Verifiable();
            mock.Protected().Setup("QueryBusinessAdministrators",
                    ItExpr.Is<Operation>(o => o == Operation.Contains),
                    ItExpr.IsAny<object>()).Verifiable();


            Mock<TaskQueryContext> ctxMock = new Mock<TaskQueryContext> { CallBase = true };
            ctxMock.Protected().Setup<TaskExpressionTranslatorBase>("InstantiateTranslator").Returns(mock.Object);
            QuerableTaskService tasks = new QuerableTaskService(ctxMock.Object); 
            var t = from task in tasks
                    where
                        task.PotentialOwners.Contains(new IdentityId().GetIdentity())
                        &&
                        task.ActualOwner==new IdentityId().GetIdentity()
                        ||
                        task.BusinessAdministrators.Contains(new IdentityId().GetIdentity())
                    select task;
            foreach (Task task in t)
            {
                //execute the query
            }
            mock.Protected().Verify("QueryPotentialOwners", Times.Once(),
                ItExpr.Is<Operation>(o => o == Operation.Contains),
                ItExpr.IsAny<object>());
            mock.Protected().Verify("QueryBusinessAdministrators", Times.Once(),
                ItExpr.Is<Operation>(o => o == Operation.Contains),
                ItExpr.IsAny<object>());
            mock.Protected().Verify("QueryActualOwner", Times.Once(),
                ItExpr.Is<Operation>(o => o == Operation.Equals),
                ItExpr.IsAny<object>());
        }

        [Test]
        public void TestBlocksPropertySearch()
        {
            Mock<TaskExpressionTranslatorBase> mock = new Mock<TaskExpressionTranslatorBase>
            {
                CallBase = true
            };
            mock.Setup(m => m.Execute()).Returns(new List<Task>());

            Mock<TaskQueryContext> ctxMock = new Mock<TaskQueryContext> { CallBase = true };
            ctxMock.Protected().Setup<TaskExpressionTranslatorBase>("InstantiateTranslator").Returns(mock.Object);
            QuerableTaskService tasks = new QuerableTaskService(ctxMock.Object);

            var t = from task in tasks
                    where
                        (task.ActualOwner == new IdentityId().GetIdentity()||task.ActualOwner==new IdentityId().GetIdentity())
                        ||
                        (task.Subject!="Test" && task.Name=="TestName")
                        &&
                        (task.Priority==Priority.High||task.Priority==Priority.Normal||task.Name.StartsWith("A"))
                    select task;
            foreach (Task task in t)
            {
                //execute the query
            }

        }

        [Test]
        public void TestSubtasksSearch()
        {
            TaskId parentTaskId=new TaskId();
            Mock<TaskExpressionTranslatorBase> mock = new Mock<TaskExpressionTranslatorBase>
                                                          {
                                                              CallBase = true
                                                          };
            mock.Setup(m => m.Execute()).Returns(new List<Task>());
            mock.Protected().Setup("QueryParentId",
                                   ItExpr.Is<Operation>(o => o == Operation.Equals),
                                   ItExpr.Is<TaskId>(o=>o.Id==parentTaskId.Id)).Verifiable();

            Mock<TaskQueryContext> ctxMock = new Mock<TaskQueryContext> { CallBase = true };
            ctxMock.Protected().Setup<TaskExpressionTranslatorBase>("InstantiateTranslator").Returns(mock.Object);
            QuerableTaskService tasks = new QuerableTaskService(ctxMock.Object);

            var t = from task in tasks
                    where
                        task.Parent.Id == parentTaskId 
                    select task;
            foreach (Task task in t)
            {
                //execute the query
            }
            mock.Protected().Verify("QueryParentId", Times.Once(),
                                    ItExpr.Is<Operation>(o => o == Operation.Equals),
                                    ItExpr.Is<TaskId>(o => o.Id == parentTaskId.Id));
        }


        private Priority GetPriority()
        {
            return Priority.Normal;
        }
    }
}
