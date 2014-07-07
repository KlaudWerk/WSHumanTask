using System;
using System.Linq;
using System.Linq.Expressions;

namespace HumanTask.Query
{
    public class TaskQueryProvider:IQueryProvider
    {
        /// <summary>
        /// Constructs an <see cref="T:System.Linq.IQueryable"/> object that can evaluate the query represented by a specified expression tree.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Linq.IQueryable"/> that can evaluate the query represented by the specified expression tree.
        /// </returns>
        /// <param name="expression">An expression tree that represents a LINQ query.</param>
        public IQueryable CreateQuery(Expression expression)
        {
            Type elementType = expression.Type.GetElementType();
            try
            {
                return (IQueryable)Activator.CreateInstance(typeof(QuerableTaskService).MakeGenericType(elementType), 
                                                            new object[] { this, expression });
            }
            catch (System.Reflection.TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        /// <summary>
        /// Constructs an <see cref="T:System.Linq.IQueryable`1"/> object that can evaluate the query represented by a specified expression tree.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Linq.IQueryable`1"/> that can evaluate the query represented by the specified expression tree.
        /// </returns>
        /// <param name="expression">An expression tree that represents a LINQ query.</param><typeparam name="TElement">The type of the elements of the <see cref="T:System.Linq.IQueryable`1"/> that is returned.</typeparam>
        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return (IQueryable<TElement>)(new QuerableTaskService(this, expression));
        }

        /// <summary>
        /// Executes the query represented by a specified expression tree.
        /// </summary>
        /// <returns>
        /// The value that results from executing the specified query.
        /// </returns>
        /// <param name="expression">An expression tree that represents a LINQ query.</param>
        public object Execute(Expression expression)
        {
            return TaskQueryContext.Execute(expression, false);
        }

        /// <summary>
        /// Executes the strongly-typed query represented by a specified expression tree.
        /// </summary>
        /// <returns>
        /// The value that results from executing the specified query.
        /// </returns>
        /// <param name="expression">An expression tree that represents a LINQ query.</param><typeparam name="TResult">The type of the value that results from executing the query.</typeparam>
        public TResult Execute<TResult>(Expression expression)
        {
            return (TResult)TaskQueryContext.Execute(expression, typeof (TResult).Name == "IEnumerable`1");
        }
    }
}