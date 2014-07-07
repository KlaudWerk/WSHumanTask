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
namespace HumanTask
{
    /// <summary>
    /// Task Status
    /// </summary>
    public enum TaskStatus
    {
        None,
        // Upon creation. Remains CREATED if there are no potential owners.
        Created,
        // Created task with multiple potential owners.
        Ready,
        // Created task with single potential owner. Work started. Actual owner set.
        Reserved,
        // Work started and task is being worked on now. Actual owner set.
        InProgress,
        // In any of its active states (Ready, Reserved, InProgress), a task can be suspended, 
        // transitioning it into the Suspended state. On resumption of the task, it transitions 
        // back to the original state from which it had been suspended.
        Suspended,
        // Successful completion of the work. One of the final states.
        Completed,
        // Unsuccessful completion of the work. One of the final states.
        Failed,
        // Unrecoverable error in human task processing. One of the final states.
        Error,
        // Task is no longer needed - skipped. This is considered a â€œgoodâ€ outcome of a task. One of the final states.
        Obsolete
    }
}