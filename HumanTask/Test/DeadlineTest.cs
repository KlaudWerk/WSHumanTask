using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using NUnit.Framework;

namespace HumanTask.Test
{
    [TestFixture]
    public class DeadlineTest
    {
        [Test]
        public void DeadlineCreateTest()
        {
            Deadline deadline=new Deadline(DeadlineType.Start);
            Escalation es1 = new Escalation("name", "descr", new FuncExpression(t =>
                                                                                    {
                                                                                        if(t.Status!=TaskStatus.Created)
                                                                                            return false;
                                                                                        DateTime nextDeadline =
                                                                                            DateTime.UtcNow +
                                                                                            new TimeSpan(1, 0, 0, 0);
                                                                                        return true;
                                                                                    }));
            Task task = new Task(new TaskId(), TaskStatus.Created, string.Empty,
                     string.Empty, Priority.Normal, false,
                     DateTime.UtcNow, null,
                     null, null, null);
            bool val= es1.Escalate(task);


        }

        
    }

    public class FuncExpression:IExpression<bool>
    {
        private readonly Func<Task, bool> _f;

        public FuncExpression(Func<Task, bool> f)
        {
            _f = f;
        }

        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data. 
        ///                 </param><param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization. 
        ///                 </param><exception cref="T:System.Security.SecurityException">The caller does not have the required permission. 
        ///                 </exception>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }

        public bool Evaluate(Task task)
        {
            return _f.Invoke(task);
        }
    }
}
