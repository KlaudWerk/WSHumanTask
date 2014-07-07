﻿/**
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
    /// <summary>
    /// Task Logger
    /// </summary>
    public interface ILoggingService
    {
        /// <summary>
        /// Gets the history.
        /// </summary>
        /// <value>The history.</value>
        IEnumerable<TaskHistoryEvent> History { get; }
        /// <summary>
        /// Logs the history entry.
        /// </summary>
        /// <param name="entry">The entry.</param>
        void LogHistoryEntry(TaskHistoryEvent entry);
        /// <summary>
        /// Gets the faults.
        /// </summary>
        /// <value>The faults.</value>
        IEnumerable<Fault> Faults { get; }
        /// <summary>
        /// Logs the fault.
        /// </summary>
        /// <param name="fault">The fault.</param>
        /// <param name="utcNow">The UTC now.</param>
        /// <param name="identity">The identity.</param>
        void LogFault(Fault fault, DateTime utcNow, IIdentity identity);
    }
}