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
using HumanTask.Hibernate.Helpers;
using NHibernate;

namespace KlaudWerk.Security.Persistence
{
    /// <summary>
    /// DAO class for an account mapping
    /// </summary>
    public class AccountMapDao
    {
        private readonly ISessionFactory _factory;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountMapDao"/> class.
        /// </summary>
        /// <param name="factory">The session.</param>
        public AccountMapDao(ISessionFactory factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Gets the by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public PersistentAccount GetById(IdentityId id)
        {
            return NHibernateHelper.RunInSession(_factory,
                s=> s.Load<PersistentAccount>(id.Id),
                (s,e)=>{});
        }

        /// <summary>
        /// Gets the by native id.
        /// </summary>
        /// <param name="nativeId">The native id.</param>
        /// <returns></returns>
        public PersistentAccount GetByNativeId(string nativeId)
        {
            return NHibernateHelper.RunInSession(_factory,
                                                 s =>s.QueryOver<PersistentAccount>().
                                                     Where(a => string.Equals(a.NativeId, nativeId)).SingleOrDefault(),
                                                 (s, e) => { });
        }

        /// <summary>
        /// Stores the specified account.
        /// </summary>
        /// <param name="account">The account.</param>
        /// <returns></returns>
        public IdentityId Store(PersistentAccount account)
        {
            return NHibernateHelper.RunInSession(_factory, s =>
                                                        {
                                                            s.SaveOrUpdate(account);
                                                            return new IdentityId(account.AccountId);
                                                        },
                                          (s, e) =>
                                              {
                                                  // no extra processing
                                              });
        }
    }
}
