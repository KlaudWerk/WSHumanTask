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
using NHibernate;

namespace HumanTask.Hibernate
{
    /// <summary>
    /// Hibernate Task Data Access object
    /// </summary>
    internal class TaskDao:ITaskVisitor
    {
        private readonly ISessionFactory _factory;

        public TaskDao(ISessionFactory factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Stores the task entity
        /// </summary>
        /// <param name="entity">The entity.</param>
        public void Store(TaskEntity entity)
        {
            Helpers.NHibernateHelper.RunInSession(_factory, s => s.SaveOrUpdate(entity), (s, e) => { });
        }

        #region Implementation of ITaskVisitor

        /// <summary>
        /// Visits the specified task.
        /// </summary>
        /// <param name="task">The task.</param>
        public void Visit(Task task)
        {
            // do nothing
        }

        /// <summary>
        /// Visits the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public void Visit(TaskEntity entity)
        {
            Store(entity);
        }

        #endregion
    }
}
