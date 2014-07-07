using FluentNHibernate.Mapping;

namespace Klaudwerk.PropertySet.Hibernate
{
    /// <summary>
    /// Mapper for Hibernate Property entry
    /// </summary>
    public class PropertyEntryMap : ClassMap<HibernatePropertyElement>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyEntryMap"/> class.
        /// </summary>
        public PropertyEntryMap()
        {
            Id(x => x.Id);
            Version(x => x.Version);
            Map(x => x.Name).Unique().Column("name");
            Map(x => x.SerializationHint).Column("hint").Not.Nullable();
            Map(x => x.IntValue).Column("intvalue").Nullable();
            Map(x => x.LongValue).Column("longvalue").Nullable();
            Map(x => x.DoubleValue).Column("doublevalue").Nullable();
            Map(x => x.DateTimeValue).Column("datetimevalue").Nullable();
            Map(x => x.StringValue).Column("stringvalue").Nullable();
            Map(x => x.RawValue).Column("raw").Nullable();
            Component(x => x.Schema, s =>
                                         {
                                             s.Map(a => a.SchemaBody, "schema_body");
                                             s.Map(a => a.SchemaType, "schema_type");
                                         });
        }
    }
}