using System;
using System.Linq.Expressions;

namespace HumanTask.Query
{
    internal static class ExpressionParser
    {
        /// <summary>
        /// Finds the innermost where.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public static MethodCallExpression FindInnermostWhere(Expression expression)
        {
            MethodCallExpression result = null;
            new MethodCallVisitor(e =>
                                      {
                                          if("where".Equals(e.Method.Name,StringComparison.InvariantCultureIgnoreCase))
                                              result = e;
                                      }).Visit(expression);
            return result;
        }
        private class MethodCallVisitor:ExpressionVisitor
        {
            private readonly Action<MethodCallExpression> _action;

            public MethodCallVisitor(Action<MethodCallExpression> action)
            {
                _action = action;
            }

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                _action.Invoke(node);
                Visit(node.Arguments[0]);
                return node;
            }
        }
    }
}