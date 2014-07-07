using System;
using System.Collections.Generic;
using System.Security.Principal;
using MongoDB.Driver;

namespace HumanTask.Mongo
{
    /// <summary>
    /// Mongo implementation of the Taks Factory
    /// </summary>
    public class MongoTaskFactory:ITaskFactory
    {
        private readonly MongoTaskDao _dao;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoTaskFactory"/> class.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <param name="taskCollectionName">Name of the task collection.</param>
        public MongoTaskFactory(MongoDatabase database,string taskCollectionName)
        {
            _dao=new MongoTaskDao(database,taskCollectionName);
        }

        /// <summary>
        /// Finds the tasks.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns></returns>
        public IEnumerable<TaskDigest> FindTasks(ITaskSearchCriteria criteria)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Finds the task.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public ITask FindTask(TaskId id)
        {
            MongoTaskCollectionElement element;
            if(_dao.TryFidnById(id.Id, out element))
            {
                return InstTask(element);
            }
            throw new ArgumentException("Task not found.");
        }

        /// <summary>
        /// Creates the task.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="name">The name.</param>
        /// <param name="subject">The subject.</param>
        /// <returns></returns>
        public ITask CreateTask(TaskId id, string name, string subject)
        {
            _dao.CreateTask(id.Id,
                TaskStatus.Created,name,subject,
                Priority.Normal,false,DateTime.UtcNow,GetCallerIdentity());
            return new MongoTask();
        }


        /// <summary>
        /// Creates the sub task.
        /// </summary>
        /// <param name="parentTaskId">The parent task id.</param>
        /// <param name="name">The name.</param>
        /// <param name="subject">The subject.</param>
        /// <returns></returns>
        public ITask CreateSubTask(TaskId parentTaskId, string name, string subject)
        {
            throw new NotImplementedException();
        }

        public IPropertySetCollectionFactory PropertySetCollectionFactory
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public IAttachmentCollectionFactory AttachmentCollectionFactory
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public IPrincipalFactory PrincipalFactory
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }


        private ITask InstTask(MongoTaskCollectionElement element)
        {
            throw new NotImplementedException();
        }

        private IIdentity GetCallerIdentity()
        {
            throw new NotImplementedException();
        }


    }
}
