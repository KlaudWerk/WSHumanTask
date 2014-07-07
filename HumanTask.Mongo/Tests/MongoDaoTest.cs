using System;
using System.Security.Principal;
using HumanTask.Mongo.Exceptions;
using MongoDB.Driver;
using Moq;
using NUnit.Framework;

namespace HumanTask.Mongo.Tests
{
    [TestFixture]
    public class MongoDaoTest
    {
        public const string TaskCollectionsDb = "ut_tasks_db";
        public const string TestCollectionName = "ut_task";
        public const string ConnStr = "mongodb://localhost";
        private MongoServer _server;
        private MongoDatabase _testDb;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            _server = MongoServer.Create(ConnStr);
            if (_server.DatabaseExists(TaskCollectionsDb))
                _server.DropDatabase(TaskCollectionsDb);
            _testDb = _server.GetDatabase(TaskCollectionsDb);
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            if (_server.DatabaseExists(TaskCollectionsDb))
                _server.DropDatabase(TaskCollectionsDb);
        }

        [SetUp]
        public void TestSetUp()
        {
            if (_testDb.CollectionExists(TestCollectionName))
                _testDb.DropCollection(TestCollectionName);
        }

        [Test]
        public void TestCreateTask()
        {
            Mock<IIdentity> mock=new Mock<IIdentity>();
            mock.SetupGet(m => m.Name).Returns("UnitTest");
            MongoTaskDao dao=new MongoTaskDao(_testDb,TestCollectionName);
            dao.CreateTask(Guid.NewGuid(), TaskStatus.Created, "test tasks", "do it", Priority.High, false,
                           DateTime.UtcNow, mock.Object);

        }

        [Test]
        [ExpectedException(typeof(DuplicateKeyException))]
        public void TestCreateTaskFailPrimaryKeyConstraint()
        {
            Mock<IIdentity> mock = new Mock<IIdentity>();
            mock.SetupGet(m => m.Name).Returns("UnitTest");
            MongoTaskDao dao = new MongoTaskDao(_testDb, TestCollectionName);
            Guid taskId = Guid.NewGuid();
            dao.CreateTask(taskId, TaskStatus.Created, "test tasks", "do it", Priority.High, false,
                           DateTime.UtcNow, mock.Object);
            dao.CreateTask(taskId, TaskStatus.Created, "test tasks", "do it", Priority.High, false,
                           DateTime.UtcNow, mock.Object);

        }

        [Test]
        public void TestFindTask()
        {
            Mock<IIdentity> mock = new Mock<IIdentity>();
            mock.SetupGet(m => m.Name).Returns("UnitTest");
            MongoTaskDao dao = new MongoTaskDao(_testDb, TestCollectionName);
            Guid taskId = Guid.NewGuid();
            dao.CreateTask(taskId, TaskStatus.Created, "test tasks", "do it", Priority.High, false,
                           DateTime.UtcNow, mock.Object);

            MongoTaskCollectionElement element;
            Assert.IsTrue(dao.TryFidnById(taskId,out element));
            Assert.IsNotNull(element);
            Assert.AreEqual(taskId.ToString(),element.Id);
            Assert.AreEqual("do it",element.Subject);
        }

        [Test]
        public void TestFindTaskFail()
        {
            Mock<IIdentity> mock = new Mock<IIdentity>();
            mock.SetupGet(m => m.Name).Returns("UnitTest");
            MongoTaskDao dao = new MongoTaskDao(_testDb, TestCollectionName);
            Guid taskId = Guid.NewGuid();
            dao.CreateTask(taskId, TaskStatus.Created, "test tasks", "do it", Priority.High, false,
                           DateTime.UtcNow, mock.Object);

            MongoTaskCollectionElement element;
            Assert.IsFalse(dao.TryFidnById(Guid.NewGuid(), out element));
            Assert.IsNull(element);
        }


    }
}
