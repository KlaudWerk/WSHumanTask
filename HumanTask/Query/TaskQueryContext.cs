using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace HumanTask.Query
{
    public enum Operation
    {
        Equals,
        NotEqual,
        Less,
        Greater,
        GreaterOrEquals,
        LessOrEquals,
        Contains,
        StartsWith,
        EndsWith
    }

    public interface IExpressionTranslator
    {
        void StartBlock();
        void EndBlock();
        void And();
        void Or();
        void Unknown();
        void Expression(Expression e);
        object Execute(bool isEnumerable);
    }
    public class TaskQueryContext
    {
        public static IExpressionTranslator Translator { get; set; }
        public static object Execute(Expression expression, bool isEnumerable)
        {
            MethodCallExpression where = ExpressionParser.FindInnermostWhere(expression);
            LambdaExpression lambda =(LambdaExpression)((UnaryExpression)Evaluator.PartialEval(where.Arguments[1])).Operand;
            TraverseExpression(lambda.Body, Translator);
            return Translator.Execute(isEnumerable);
        }

        /// <summary>
        /// Traverses the extpression tree 
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <param name="translator">The expression translator.</param>
        private static void TraverseExpression(Expression exp,IExpressionTranslator translator)
        {
            BinaryExpression bexp = exp as BinaryExpression;
            if(bexp==null)
            {
                translator.Expression(exp);
                return;
            }
            if (bexp.Left is BinaryExpression)
            {
                translator.StartBlock();
                TraverseExpression(bexp.Left as BinaryExpression, translator);
                switch (bexp.NodeType)
                {
                    case ExpressionType.AndAlso:
                        translator.And();
                        break;
                    case ExpressionType.OrElse:
                        translator.Or();
                        break;
                }
                if (bexp.Right is BinaryExpression)
                {
                    TraverseExpression(bexp.Right as BinaryExpression, translator);
                }
                else
                {
                   translator.Expression(bexp.Right);
                }
                translator.EndBlock();
            }
            else
            {
                switch (bexp.NodeType)
                {
                    case ExpressionType.AndAlso:
                        translator.And();
                        break;
                    case ExpressionType.OrElse:
                        translator.Or();
                        break;
                }
                translator.Expression(bexp);
            }
        }
    }

    /// <summary>
    /// Base expression translator for Task class
    /// </summary>
    public abstract class TaskExpressionTranslatorBase:ExpressionVisitor,IExpressionTranslator
    {
        private readonly Dictionary<string, Action<Operation, object>> _simplePropertySetters;

        protected TaskExpressionTranslatorBase()
        {
            _simplePropertySetters = new Dictionary<string, Action<Operation, object>>
                                         {
                                             {"Id",(o,v)=>QueryId(o,v)},
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
                                         };
        }

        protected abstract void QueryName(Operation operation, object o);
        protected abstract void QuerySubject(Operation operation, object o);
        protected abstract void QueryTaskOutcome(Operation operation, object o);
        protected abstract void QueryId(Operation operation, object o);
        protected abstract void QueryCompleted(Operation operation, object o);
        protected abstract void QueryStarted(Operation operation, object o);
        protected abstract void QueryCreated(Operation operation, object o);
        protected abstract void QueryStatus(Operation operation, object value);
        protected abstract void QueryPriority(Operation operation, object value);
        protected abstract void QueryActualOwner(Operation operation, object value);
        protected abstract void QueryInitiator(Operation operation, object value);

        public virtual void StartBlock()
        {
            
        }

        public virtual void EndBlock()
        {
            
        }

        public virtual void And()
        {
            
        }

        public virtual void Or()
        {
            
        }

        public virtual void Unknown()
        {
            throw new ArgumentException("Unknown node type.");
        }

        public virtual void Expression(Expression e)
        {
            Visit(e);
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
            if(_simplePropertySetters.TryGetValue(lv.Member.Name,out setAction))
            {
                setAction.Invoke(TranslateOperation(node.NodeType),cv.Value);
            }
            return node;
        }

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

        public abstract object Execute(bool isEnumerable);

        class LeftPartVisitor:ConstantVisitor
        {
            public MemberInfo Member { get; private set; }
            protected override Expression VisitMember(MemberExpression node)
            {
                Visited = true;
                Member = node.Member;
                return node;
            }

        }
        class ConstantVisitor:ExpressionVisitor
        {
            public bool Visited { get; protected set; }
            public Type Type { get; private set; }
            public object Value { get; private set; }
            protected override Expression VisitConstant(ConstantExpression node)
            {
                Visited = true;
                Value = node.Value;
                Type = node.Type;
                return node;
            }
        }
    }
    
}