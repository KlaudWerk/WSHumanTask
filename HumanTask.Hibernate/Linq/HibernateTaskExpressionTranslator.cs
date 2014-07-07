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
using HumanTask.Linq;
using NHibernate;
using NHibernate.Criterion;

namespace HumanTask.Hibernate.Linq
{
    /// <summary>
    /// Creates and executes NHibernate Criteria Query
    /// </summary>
    internal class HibernateTaskExpressionTranslator :TaskExpressionTranslatorBase
    {
        private static readonly Dictionary<Operation, Action<HibernateTaskExpressionTranslator, string, object>>
            _opMap = new Dictionary<Operation, Action<HibernateTaskExpressionTranslator, string, object>>
                         {
                             {Operation.EndsWith,(t,f,v)=>t.AddExpression(Restrictions.Like(f, v == null ? 
                                                                          null : v.ToString(), MatchMode.End))},
                             {Operation.Equals,(t,f,v)=>t.AddExpression(Restrictions.Eq(f,v))},
                             {Operation.Greater,(t,f,v)=>t.AddExpression(Restrictions.Gt(f, v))},
                             {Operation.GreaterOrEquals,(t,f,v)=>t.AddExpression(Restrictions.Ge(f, v))},
                             {Operation.In,(t,f,v)=>{}},
                             {Operation.Less,(t,f,v)=>t.AddExpression(Restrictions.Lt(f, v))},
                             {Operation.LessOrEquals,(t,f,v)=>t.AddExpression(Restrictions.Le(f, v))},
                             {Operation.Like,(t,f,v)=>t.AddExpression(Restrictions.Like(f, v))},
                             {Operation.NotEqual,(t,f,v)=>t.AddExpression(Restrictions.Not(Restrictions.Eq(f,v)))},
                             {Operation.NotIn,(t,f,v)=>{}},
                             {Operation.StartsWith,(t,f,v)=>t.AddExpression(Restrictions.Like(f, v == null ? null : v.ToString(), MatchMode.Start))}
                         };

        private readonly ISessionFactory _sessionFactory;
        private AbstractCriterion _criterion;
        private Func<AbstractCriterion, AbstractCriterion, AbstractCriterion> _join;
        private ICriteria _query;
        /// <summary>
        /// Initializes a new instance of the <see cref="HibernateTaskExpressionTranslator"/> class.
        /// </summary>
        /// <param name="sessionFactory">The session factory.</param>
        public HibernateTaskExpressionTranslator(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
            _join = OnEmptyJoin;
            _query = Helpers.NHibernateHelper.RunInSession(_sessionFactory,
                            s => s.CreateCriteria(typeof(TaskEntity)),
                            (s, e) => { throw e; });
            
        }
        /// <summary>
        /// Executes the created query
        /// </summary>
        /// <returns></returns>
        public override IList<Task> Execute()
        {
            return Helpers.NHibernateHelper.RunInSession(_sessionFactory,
                s =>
                {
                    ICriteria criteria = s.CreateCriteria(typeof(TaskEntity));
                    if(_criterion!=null)
                        criteria.Add(_criterion);
                    IList<TaskEntity> list = criteria.List<TaskEntity>();
                    return new TaskListProxy(list);
                },
                (s, e) => { throw e; });
        }

        protected override void OnStartBlock()
        {
            _join = OnEmptyJoin;
        }


        protected override void OnEndBlock()
        {
            _join = OnEmptyJoin;
        }

        protected override void OnAnd()
        {
            _join = OnEndJoin;
        }

        protected override void OnOr()
        {
            _join = OnOrJoin;
        }

        private static AbstractCriterion OnEmptyJoin(AbstractCriterion current, AbstractCriterion next)
        {
            return next;
        }

        private static AbstractCriterion OnEndJoin(AbstractCriterion current, AbstractCriterion next)
        {
            return current == null ? next : Restrictions.And(current, next);
        }

        private static AbstractCriterion OnOrJoin(AbstractCriterion current, AbstractCriterion next)
        {
            return current == null ? next : Restrictions.Or(current, next);                
        }

        private static Action<HibernateTaskExpressionTranslator, string, object> GetAction(Operation operation)
        {
            Action<HibernateTaskExpressionTranslator, string, object> action;
            if (!_opMap.TryGetValue(operation, out action))
                throw new ArgumentException(operation.ToString());
            return action;
        }

        protected override void QueryId(Operation operation, object o)
        {
            GetAction(operation).Invoke(this, "TaskId", o);
        }

        protected override void QueryParentId(Operation operation, object o)
        {
            GetAction(operation).Invoke(this, "ParentId", o);
        }

        protected override void QueryName(Operation operation, object o)
        {
            OnSearchString("Name", operation, o);
        }

        protected override void QuerySubject(Operation operation, object o)
        {
            OnSearchString("Subject",operation,o);
        }

        protected override void QueryTaskOutcome(Operation operation, object o)
        {
            throw new NotImplementedException();
        }

        protected override void QueryCompleted(Operation operation, object o)
        {
            throw new NotImplementedException();
        }

        protected override void QueryStarted(Operation operation, object o)
        {
            throw new NotImplementedException();
        }

        protected override void QueryCreated(Operation operation, object o)
        {
            throw new NotImplementedException();
        }

        protected override void QueryStatus(Operation operation, object value)
        {
            GetAction(operation).Invoke(this, "Status", (TaskStatus)value);
        }

        protected override void QueryPriority(Operation operation, object value)
        {
            GetAction(operation).Invoke(this, "Priority", (Priority)value);
        }

        protected override void QueryActualOwner(Operation operation, object value)
        {
            throw new NotImplementedException();
        }

        protected override void QueryInitiator(Operation operation, object value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Queries the potential owners.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="value">The value.</param>
        protected override void QueryPotentialOwners(Operation operation, object value)
        {
            
            throw new NotImplementedException();
        }

        protected override void QueryExcludedOwners(Operation operation, object value)
        {
            throw new NotImplementedException();
        }

        protected override void QueryBusinessAdministrators(Operation operation, object value)
        {
            throw new NotImplementedException();
        }

        protected override void QueryStakeholders(Operation operation, object value)
        {
            throw new NotImplementedException();
        }

        protected override void QueryRecepients(Operation operation, object value)
        {
            throw new NotImplementedException();
        }

        protected override void QueryPotentialDelegatees(Operation operation, object value)
        {
            throw new NotImplementedException();
        }


        #region Private Criteria Methods

        private void AddExpression(AbstractCriterion expression)
        {
            _criterion = _join.Invoke(_criterion, expression);
        }

        private void OnSearchString(string field, Operation operation, object value)
        {
            if (operation == Operation.Contains)
                AddExpression(Restrictions.Like(field, value == null ? null : value.ToString(), MatchMode.Anywhere));
            else
                GetAction(operation).Invoke(this, field, value);
        }

        #endregion
    }
}
