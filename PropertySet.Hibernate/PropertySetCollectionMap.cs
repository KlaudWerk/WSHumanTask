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

namespace Klaudwerk.PropertySet.Hibernate
{
    /// <summary>
    /// Map class for PropertySet collection
    /// </summary>
    public class PropertySetCollectionMap:ClassMapping<PropertyElementsCollection>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertySetCollectionMap"/> class.
        /// </summary>
        public PropertySetCollectionMap()
        {
            Table("propertyset");
            Id(x => x.Id, map =>
                              {
                                  map.Generator(Generators.Assigned);
                                  map.Column("propertyset_id");
                              });

            Map(x => x.Elements, map =>
                                     {
                                         map.Access(Accessor.Property);
                                         map.Key(x => x.Column("propertyset_id"));
                                         map.Table("propertyset_elements");
                                     },
                r => r.Component(x =>
                                     {
                                         x.Property(p => p.SerializationHint,m=>m.NotNullable(true));
                                         x.Property(p => p.IntValue);
                                         x.Property(p => p.LongValue);
                                         x.Property(p => p.DoubleValue);
                                         x.Property(p => p.DateTimeValue);
                                         x.Property(p => p.StringValue);
                                         x.Property(p => p.RawValue,m=>m.Column(c=>c.SqlType("varbinary(max)")));
                                         x.Property(p=>p.ValueType);
                                         x.Component(s => s.Schema,map=>
                                                                     {
                                                                         map.Property(p=>p.SchemaBody);
                                                                         map.Property(p=>p.SchemaType);
                                                                     });
                                     }));
        }
    }
}