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
using HumanTask.Security;
using KlaudWerk.Security;

namespace HumanTask
{
    /// <summary>
    /// Task History Entry stores changes in essential task instance data
    /// such as :
    ///  - Task Status
    ///  - Task Priority
    ///  - Actual Owner
    /// </summary>
    [Serializable]
    public class TaskHistoryEvent : TaskComment
    {
        /// <summary>
        /// Gets or sets the name of the history event.
        /// </summary>
        /// <value>The event.</value>
        public string Event { get;  set; }
        /// <summary>
        /// Gets or sets the old priority.
        /// </summary>
        /// <value>The old priority.</value>
        public Priority OldPriority { get;  set; }
        /// <summary>
        /// Gets or sets the new priority.
        /// </summary>
        /// <value>The new priority.</value>
        public Priority NewPriority { get;  set; }
        /// <summary>
        /// Gets the old status.
        /// </summary>
        public TaskStatus OldStatus { get;  set; }
        /// <summary>
        /// Gets the new status.
        /// </summary>
        public TaskStatus NewStatus { get;  set; }
        /// <summary>
        /// Gets or sets the original owner.
        /// </summary>
        /// <value>The original owner.</value>
        public IdentityId StartOwner { get;  set; }
        /// <summary>
        /// Gets or sets the actual owner.
        /// </summary>
        /// <value>The actual owner.</value>
        public IdentityId EndOwner { get;  set; }
    }
}