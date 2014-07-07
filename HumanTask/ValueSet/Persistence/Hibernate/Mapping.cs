using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using HumanTask.ValueSet.Serialization;

namespace HumanTask.ValueSet.Persistence.Hibernate
{
    /// <summary>
    /// Property Collection Mapping class
    /// </summary>
    public class PropertyCollectionMap : ClassMap<PropertyCollection>
    {
        public PropertyCollectionMap()
        {
            Id(x => x.Id);
            Version(x => x.Version);
            Map(x => x.Name);
            HasMany(x => x.Values);
        }
    }

    public class PropertyEntryMap : ClassMap<PropertyElement>
    {
        public PropertyEntryMap()
        {
            Map(x => x.SchemaBody);
            Map(x => x.ValString).Nullable();
            Map(x => x.ValInt).Nullable();
            Map(x => x.ValLong).Nullable();
            Map(x => x.ValDateTime).Nullable();
            Map(x => x.ValDouble).Nullable();
            Map(x => x.ValRaw).Nullable();
        }
    }

}
