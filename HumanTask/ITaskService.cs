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
using System.Security.Principal;

namespace HumanTask
{
    public interface ITaskService
    {
        IEnumerable<TaskDigest> GetTasks(ITaskSearchCriteria criteria);
        TaskDigest GetTaskDigest(TaskId id);
        TaskId CreateTask(string name, string subject);
        TaskId CreateSubTask(TaskId parentTaskId, string name, string subject);
        IEnumerable<TaskComment> GetComments(TaskId task);
        void AddComment(TaskId id, string comment);
        #region Task Life cycle Operations

        void Claim(TaskId task);
        void Start(TaskId task);
        void Stop(TaskId task);
        void Release(TaskId task);
        void Suspend(TaskId task);
        void SuspendUntilNextDate(TaskId task,DateTime nextTime);
        void SuspendUntilTimePeriod(TaskId task, TimeSpan timePeriod);
        void Resume(TaskId task);
        Outcome Complete(TaskId task);
        void Remove(TaskId task);
        void Fail(TaskId task);
        void Skip(TaskId task);
        void Forward(TaskId task, IIdentity target);
        void Delegate(TaskId task, IIdentity target, Priority priority);
        void SetPriority(TaskId task, Priority priority);
        #endregion

        #region Task Administrator operations

        void Activate(TaskId task);
        void Nominate(TaskId task,IIdentity target);
        void SetGenericHumanRole(TaskId task, IIdentity identity, HumanRoles role);

        #endregion


    }
}