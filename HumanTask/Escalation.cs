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
using System.Collections.Generic;
using System.Security.Principal;

namespace HumanTask
{
    /// <summary>
    /// The Escalation class
    /// </summary>
    public class Escalation
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }
        private readonly IExpression<bool>_expression;
        /// <summary>
        /// Gets or sets the notifications.
        /// </summary>
        /// <value>The notifications.</value>
        public ICollection<INotification> Notifications { get; private set; }
        /// <summary>
        /// Gets or sets the reassignments.
        /// </summary>
        /// <value>The reassignments.</value>
        public ICollection<IIdentity> Reassignments { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Escalation"/> class.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="expression">The expression.</param>
        public Escalation(
            string name,
            string description,
            IExpression<bool> expression)
        {
            Name = name;
            Description = description;
            _expression = expression;
            Notifications = new List<INotification>();
            Reassignments = new List<IIdentity>();
        }
        /// <summary>
        /// Escalates this instance.
        /// </summary>
        /// <returns></returns>
        public bool Escalate(Task task)
        {
            bool result = _expression.Evaluate(task);
            if (result)
            {
                foreach (INotification notification in Notifications)
                    notification.Deliver(task);
                foreach(IIdentity identity in Reassignments)
                    task.PotentialOwners.Add(identity);
            }
            return result;
        }
    }
}