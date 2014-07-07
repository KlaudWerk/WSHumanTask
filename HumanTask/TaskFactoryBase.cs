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
using System.Security.Principal;
using System.Text;
using System.Threading;
using Klaudwerk.PropertySet;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;

namespace HumanTask
{
    public class TaskFactory:ITaskFactory
    {
        /// <summary>
        /// Finds the task by its primary key.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public Task FindTask(TaskId id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates the task.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="subject">The subject.</param>
        /// <returns></returns>
        public Task CreateTask(string name, string subject)
        {
            return CreateTask(name, subject, Priority.Normal, false);
        }

        /// <summary>
        /// Creates the task.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="priority">The priority.</param>
        /// <param name="isSkippable">if set to <c>true</c> [is skippable].</param>
        /// <returns></returns>
        public Task CreateTask(string name, string subject, Priority priority, bool isSkippable)
        {
            TaskId taskId = new TaskId();
            return new Task(taskId,
                            TaskStatus.Created, name,
                            subject,
                            priority,
                            isSkippable,
                            DateTime.UtcNow,
                            Thread.CurrentPrincipal.Identity,
                            null, null, null)
                       {
                           ProcesingData =
                               ServiceLocator.Current.GetInstance<IPropertySetCollectionFactory>().Create(taskId),
                           LoggingService = ServiceLocator.Current.GetInstance<ILoggingServiceFactory>().Create(taskId)
                       };

        }
    }
    /// <summary>
     /// 
     /// </summary>
    public interface IPropertySetCollectionFactory
    {
        IPropertySetCollection Create(TaskId taskId);
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IAttachmentCollectionFactory
    {
        IAttachmentCollection Create(TaskId taskId);
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IPrincipalFactory
    {
        IPrincipal GetPrincipal(TaskId taskId);
    }

    public interface ILoggingServiceFactory
    {
        ILoggingService Create(TaskId taskId);
    }



}
