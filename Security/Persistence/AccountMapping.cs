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
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace KlaudWerk.Security.Persistence
{
    public class AccountMapping:ClassMapping<PersistentAccount>
    {
        public AccountMapping()
        {
            Table("AccountMap");
            Id(x => x.AccountId, m =>
                                    {
                                        m.Generator(Generators.Guid);
                                        m.Column("mapped_id");
                                    });
            Property(x=>x.NativeId,m=>
                                       {
                                           m.Column("native_id");
                                           m.NotNullable(true);
                                           m.Index("IDX_NATIVE_ID");
                                       });
            Property(x=>x.AuthSystem,m=>m.Column("auth_type"));
            Property(x=>x.AccountType,m=>m.Column("account_type"));
            Property(x=>x.DisplayName,m=>m.Column("display_name"));
            Property(x=>x.DistinguishedName,m=>m.Column("account_name"));
        }
    }
}
