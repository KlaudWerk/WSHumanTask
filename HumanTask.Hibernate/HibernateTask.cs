using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;

namespace HumanTask.Hibernate
{
    public class HibernateTask:Task
    {
        public HibernateTask(TaskId id, 
            TaskStatus status, 
            string name, 
            string subject, 
            Priority priority, 
            bool isSkippable, 
            DateTime created, 
            IIdentity initiator, 
            DateTime? started, 
            DateTime? completed, 
            IIdentity actualOwner) : 
            base(id, status, name, subject, priority, isSkippable, created, initiator, started, completed, actualOwner)
        {
        }

        /// <summary>
        /// Gets the parent task.
        /// </summary>
        /// <value>The parent.</value>
        public override ITask Parent
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Adds the potential owner.
        /// </summary>
        /// <param name="target">The target.</param>
        protected override void AddPotentialOwner(IIdentity target)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the current logged-in principal.
        /// </summary>
        /// <returns></returns>
        protected override IPrincipal GetCurrentPrincipal()
        {
            throw new NotImplementedException();
        }
    }
}
