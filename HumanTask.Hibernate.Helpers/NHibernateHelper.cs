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
using System.Data;
using System.Reflection;
using log4net;
using NHibernate;
using NHibernate.Context;

namespace HumanTask.Hibernate.Helpers
{
    public class NHibernateHelper
    {
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// Runs the in session.
        /// </summary>
        /// <param name="sessionFactory">The session factory.</param>
        /// <param name="block">The block.</param>
        /// <param name="exception">The exception.</param>
        public static void RunInSession(ISessionFactory sessionFactory,
            Action<ISession> block, Action<ISession, Exception> exception)
        {
            RunInSession<object>(sessionFactory, s =>
                                                     {
                                                         block.Invoke(s);
                                                         return null;
                                                     },
                                 exception.Invoke);
        }
        /// <summary>
        /// Runs the in session.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sessionFactory">The session factory.</param>
        /// <param name="block">The block.</param>
        /// <param name="exception">The exception.</param>
        /// <returns></returns>
        public static T RunInSession<T>(
            ISessionFactory sessionFactory,
            Func<ISession, T> block, Action<ISession, Exception> exception)
        {
            bool needClosing = false;
            bool trxCreated = false;
            ISession session;
            ITransaction transaction;
            try
            {
                session = sessionFactory.GetCurrentSession();
                transaction = session.Transaction;
                if (transaction == null)
                {
                    transaction = session.BeginTransaction(IsolationLevel.Serializable);
                    trxCreated = true;
                }
            }
            catch (Exception)
            {
                needClosing = true;
                session = sessionFactory.OpenSession();
                transaction = session.BeginTransaction(IsolationLevel.Serializable);
                trxCreated = true;
                CurrentSessionContext.Bind(session);
            }

            try
            {
                T value = block.Invoke(session);
                if (trxCreated)
                    transaction.Commit();
                return value;
            }
            catch (Exception ex)
            {
                try
                {
                    _logger.Error(ex);
                    exception.Invoke(session, ex);
                    throw;
                }
                finally
                {

                    if (trxCreated)
                        transaction.Rollback();
                }
            }
            finally
            {
                if (needClosing)
                {
                    CurrentSessionContext.Unbind(sessionFactory);
                    session.Close();
                }
            }

        }

    }
}
