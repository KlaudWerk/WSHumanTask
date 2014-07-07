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
using System.Linq.Expressions;

namespace HumanTask.Linq
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