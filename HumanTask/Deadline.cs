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

namespace HumanTask
{
    /// <summary>
    /// The deadline type
    /// </summary>
    public enum DeadlineType
    {
        /// <summary>
        /// Start deadline
        /// </summary>
        Start ,
        /// <summary>
        /// Completion deadline
        /// </summary>
        Completion
    }
    /// <summary>
    /// The Task's Deadline container
    /// </summary>
    public class Deadline
    {
        /// <summary>
        /// Gets or sets the type of the deadline.
        /// </summary>
        /// <value>The type of the deadline.</value>
        public DeadlineType DeadlineType
        {
            get; private set;
        }
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get; private set;
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description
        {
            get; 
            set;
        }
        /// <summary>
        /// Gets or sets the deadline date time.
        /// </summary>
        /// <value>The deadline date time.</value>
        public DateTime Date
        {
            get;  
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Deadline"/> was escalated.
        /// </summary>
        /// <value><c>true</c> if escalated; otherwise, <c>false</c>.</value>
        public bool Escalated { get; set; }
        /// <summary>
        /// Gets or sets the escalations.
        /// </summary>
        /// <value>The escalations.</value>
        public ICollection<Escalation> Escalations { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Deadline"/> class.
        /// </summary>
        /// <param name="deadlineType"></param>
        /// <param name="name">the deadline name</param>
        public Deadline(
            DeadlineType deadlineType,
            string name
            )
        {
            DeadlineType = deadlineType;
            Name = name;
            Escalations = new List<Escalation>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Deadline"/> class.
        /// </summary>
        public Deadline(DeadlineType deadlineType):this(deadlineType , string.Empty)
        {
            
        }
    }
}