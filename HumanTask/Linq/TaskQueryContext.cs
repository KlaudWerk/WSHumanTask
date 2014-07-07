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

namespace HumanTask.Linq
{
    /// <summary>
    /// The Task Query Context
    /// </summary>
    public abstract class TaskQueryContext
    {
        
        /// <summary>
        /// Executes the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public object Execute(Expression expression)
        {
            MethodCallExpression where = ExpressionParser.FindInnermostWhere(expression);
            LambdaExpression lambda =
                (LambdaExpression) ((UnaryExpression) Evaluator.PartialEval(where.Arguments[1])).Operand;
            TaskExpressionTranslatorBase translator = InstantiateTranslator();
            TraverseExpression(lambda.Body, translator);
            IList<Task> result = translator.Execute();
            return (typeof (IQueryable).IsAssignableFrom(expression.Type))
                       ? result.AsQueryable()
                       : ExecuteLinqExtensionMethod(result.AsQueryable(), expression);
        }

        /// <summary>
        /// Creates the new instance of an translator.
        /// </summary>
        /// <returns></returns>
        protected abstract TaskExpressionTranslatorBase InstantiateTranslator();

        /// <summary>
        /// Executes the one of the LINQ scalar extension methods
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        private static object ExecuteLinqExtensionMethod(IQueryable result, Expression expression)
        {
            MethodCallExpression me = expression as MethodCallExpression;
            if (me != null && !string.Equals("where", me.Method.Name, StringComparison.InvariantCultureIgnoreCase))
            {
                List<object> arguments = new List<object>{result};
                arguments.AddRange(me.Arguments.Where(
                    argumentExpression => !typeof (IQueryable).IsAssignableFrom(argumentExpression.Type)).
                    OfType<ConstantExpression>().Select(ce => ce.Value));
                return me.Method.Invoke(null, arguments.ToArray());
            }
            return result;
        }

        /// <summary>
        /// Traverses the extpression tree 
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <param name="translator">The expression translator.</param>
        private static void TraverseExpression(Expression exp,TaskExpressionTranslatorBase translator)
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
                translator.TranslateNodeType(bexp.NodeType);
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
                if (bexp.Right is BinaryExpression)
                {
                    translator.Expression(bexp.Left);
                    translator.TranslateNodeType(bexp.NodeType);
                    TraverseExpression(bexp.Right, translator);
                }
                else
                {
                    translator.Expression(bexp);
                }
            }
        }
    }
}