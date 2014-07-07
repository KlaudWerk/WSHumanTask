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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using log4net;

namespace HumanTask.Linq
{
    /// <summary>
    /// Base expression translator for Task class
    /// </summary>
    public abstract class TaskExpressionTranslatorBase:ExpressionVisitor
    {
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly Dictionary<string,Operation> 
            _methodToOperationMap=new Dictionary<string, Operation> 
                                        {
                                            {"Contains",Operation.Contains},                                            
                                            {"Like",Operation.Like},
                                            {"StartsWith",Operation.StartsWith},
                                            {"EndsWith",Operation.EndsWith},
                                            {"In",Operation.In},
                                            {"NotIn",Operation.NotIn},
                                        };
        private readonly Dictionary<string, Action<Operation, object>> _simplePropertySetters;

        protected TaskExpressionTranslatorBase()
        {
            _simplePropertySetters = new Dictionary<string, Action<Operation, object>>
                                         {
                                             {"Id",(o,v)=>QueryId(o,v)},
                                             {"Parent.Id",(o,v)=>QueryParentId(o,v)},
                                             {"Priority",(o,v)=>QueryPriority(o,v)},
                                             {"Status",(o,v)=>QueryStatus(o,v)},
                                             {"ActualOwner",(o,v)=>QueryActualOwner(o,v)},
                                             {"Initiator",(o,v)=>QueryInitiator(o,v)},
                                             {"Created",(o,v)=>QueryCreated(o,v)},
                                             {"Started",(o,v)=>QueryStarted(o,v)},
                                             {"Completed",(o,v)=>QueryCompleted(o,v)},
                                             {"TaskOutcome",(o,v)=>QueryTaskOutcome(o,v)},
                                             {"Subject",(o,v)=>QuerySubject(o,v)},
                                             {"Name",(o,v)=>QueryName(o,v)},
                                             {"PotentialOwners",(o,v)=>QueryPotentialOwners(o,v)},
                                             {"BusinessAdministrators",(o,v)=>QueryBusinessAdministrators(o,v)},
                                             {"ExcludedOwners",(o,v)=>QueryExcludedOwners(o,v)},
                                             {"Stakeholders",(o,v)=>QueryStakeholders(o,v)},
                                             {"Recepients",(o,v)=>QueryRecepients(o,v)},
                                             {"PotentialDelegatees",(o,v)=>QueryPotentialDelegatees(o,v)},
                                             
                                         };
        }

        #region Public Methods
        public void TranslateNodeType(ExpressionType nodeType)
        {
            switch (nodeType)
            {
                case ExpressionType.AndAlso:
                    And();
                    break;
                case ExpressionType.OrElse:
                    Or();
                    break;
            }
        }
        public void StartBlock()
        {
            _logger.DebugEx(() => "(");
            OnStartBlock();
        }

        public void EndBlock()
        {
            _logger.DebugEx(() => ")");
            OnEndBlock();
        }

        public void And()
        {
            _logger.DebugEx(() => "AND");
            OnAnd();
        }

        public void Or()
        {
            _logger.DebugEx(() => "OR");
            OnOr();
        }

        public void Unknown()
        {
            OnUnknown();
        }

        /// <summary>
        /// Evaluate the expression using visitors
        /// </summary>
        /// <param name="e">The e.</param>
        public virtual void Expression(Expression e)
        {
            Visit(e);
        }
        #endregion

        protected abstract void QueryName(Operation operation, object o);
        protected abstract void QuerySubject(Operation operation, object o);
        protected abstract void QueryTaskOutcome(Operation operation, object o);
        protected abstract void QueryId(Operation operation, object o);
        protected abstract void QueryParentId(Operation operation, object o);
        protected abstract void QueryCompleted(Operation operation, object o);
        protected abstract void QueryStarted(Operation operation, object o);
        protected abstract void QueryCreated(Operation operation, object o);
        protected abstract void QueryStatus(Operation operation, object value);
        protected abstract void QueryPriority(Operation operation, object value);
        protected abstract void QueryActualOwner(Operation operation, object value);
        protected abstract void QueryInitiator(Operation operation, object value);
        protected abstract void QueryPotentialOwners(Operation operation, object value);
        protected abstract void QueryExcludedOwners(Operation operation, object value);
        protected abstract void QueryBusinessAdministrators(Operation operation, object value);
        protected abstract void QueryStakeholders(Operation operation, object value);
        protected abstract void QueryRecepients(Operation operation, object value);
        protected abstract void QueryPotentialDelegatees(Operation operation, object value);


        protected abstract void OnStartBlock();
        protected abstract void OnEndBlock();
        protected abstract void OnAnd();
        protected abstract void OnOr();
        protected virtual void OnUnknown()
        {
            throw new ArgumentException("Unknown node type.");
        }
        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.MethodCallExpression"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
        /// </returns>
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            Operation operation;
            if (!_methodToOperationMap.TryGetValue(node.Method.Name, out operation))
            {
                throw new ArgumentException(string.Format("Method '{0}' is not supported.", node.Method.Name));
            }
            List<object> values = new List<object>();
            ConstantVisitor cv = new ConstantVisitor();
            foreach (Expression expression in node.Arguments)
            {
                cv.Visit(expression);
                if (!cv.Visited)
                    throw new ArgumentException("Cannot evaluate:" + expression);
                values.Add(cv.Value);
            }
            if (node.Object != null)
            {
                MemberExpression me = (MemberExpression) node.Object;
                Action<Operation, object> setAction;
                _logger.DebugEx(() => string.Format("{0} {1} {2}", me.Member.Name,operation,
                                        string.Join(",", values.Select(v => "" + v).ToArray())));

                if (_simplePropertySetters.TryGetValue(me.Member.Name, out setAction))
                {
                    setAction.Invoke(operation, values.Count==1?values[0]:values.ToArray());
                }
            }
            else
                throw new ArgumentException("Unknown Property.");
            return node;
        }


        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.BinaryExpression"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
        /// </returns>
        protected override Expression VisitBinary(BinaryExpression node)
        {
            LeftPartVisitor lv=new LeftPartVisitor();
            lv.Visit(node.Left);
            ConstantVisitor cv=new ConstantVisitor();
            cv.Visit(node.Right);
            if(!cv.Visited)
                throw new ArgumentException("Cannot evaluate:"+node.Right);
            Action<Operation, object> setAction;
            if(_simplePropertySetters.TryGetValue(lv.GetPropertiesPath(),out setAction))
            {
                setAction.Invoke(TranslateOperation(node.NodeType),cv.Value);
            }
            _logger.DebugEx(() => string.Format("{0} {1} {2}", lv.GetPropertiesPath(), TranslateOperation(node.NodeType), cv.Value));
            return node;
        }

        /// <summary>
        /// Translates the operation.
        /// </summary>
        /// <param name="nodeType">Type of the node.</param>
        /// <returns></returns>
        private static Operation TranslateOperation(ExpressionType nodeType)
        {
            switch (nodeType)
            {
                case ExpressionType.Equal:
                    return Operation.Equals;
                case ExpressionType.NotEqual:
                    return Operation.NotEqual;
                case ExpressionType.LessThan:
                    return Operation.Less;
                case ExpressionType.LessThanOrEqual:
                    return Operation.LessOrEquals;
                case ExpressionType.GreaterThan:
                    return Operation.Greater;
                case ExpressionType.GreaterThanOrEqual:
                    return Operation.GreaterOrEquals;
            }
            return Operation.Equals;
        }

        /// <summary>
        /// Executes the translation expression
        /// </summary>
        /// <returns></returns>
        public abstract IList<Task> Execute();

        /// <summary>
        /// The vistor helper
        /// </summary>
        class LeftPartVisitor:ConstantVisitor
        {
            /// <summary>
            /// Gets or sets the members.
            /// </summary>
            /// <value>The members.</value>
            private List<MemberInfo> Members { get; set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="LeftPartVisitor"/> class.
            /// </summary>
            public LeftPartVisitor()
            {
                Members=new List<MemberInfo>();
            }

            /// <summary>
            /// Visits the children of the <see cref="T:System.Linq.Expressions.MemberExpression"/>.
            /// </summary>
            /// <param name="node">The expression to visit.</param>
            /// <returns>
            /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
            /// </returns>
            protected override Expression VisitMember(MemberExpression node)
            {
                Visited = true;
                MemberExpression me = node.Expression as MemberExpression;
                if (me != null)
                    VisitMember(me);
                Members.Add(node.Member);
                return node;
            }

            /// <summary>
            /// Gets the properties path.
            /// </summary>
            /// <returns></returns>
            public string GetPropertiesPath()
            {

                return string.Join(".", Members.Select(m => m.Name).ToArray());
            }

        }
        /// <summary>
        /// The constant visitor helper
        /// </summary>
        class ConstantVisitor:ExpressionVisitor
        {
            public bool Visited { get; protected set; }
            public object Value { get; private set; }
            /// <summary>
            /// Visits the <see cref="T:System.Linq.Expressions.ConstantExpression"/>.
            /// </summary>
            /// <param name="node">The expression to visit.</param>
            /// <returns>
            /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
            /// </returns>
            protected override Expression VisitConstant(ConstantExpression node)
            {
                Visited = true;
                Value = node.Value;
                return node;
            }
        }
    }
}