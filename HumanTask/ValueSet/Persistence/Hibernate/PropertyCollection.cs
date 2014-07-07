using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;

namespace HumanTask.ValueSet.Persistence.Hibernate
{
    public class PropertyCollection
    {
        public virtual long Id { get; private set; }
        public virtual string Name { get; set; }
        public virtual int Version { get; set; }
        public virtual IDictionary<string, PropertyEntry> Values { get; private set; }
    }

    public class PropertyCollectionMap:ClassMap<PropertyCollection>
    {
        public PropertyCollectionMap()
        {
            Id(x => x.Id);
            Version(x => x.Version);
            Map(x => x.Name);
            HasMany(x => x.Values);
        }
    } 

    public class PropertyEntry
    {
        public virtual long Id { get; set; }
        public virtual string Name { get; set; }
        public virtual int Version { get; set; }
        public virtual ClassDescriptor EntryClass { get; set; }
        public virtual ClassDescriptor SchemaClass { get; set; }
        public virtual string SchemaBody { get; set; }
        public virtual string ValString { get; set; }
        public virtual int? ValInt { get; set; }
        public virtual long? ValLong { get; set; }
        public virtual DateTime? ValDateTime { get; set; }
        public virtual double? ValDouble { get; set; }
        public virtual byte[] ValRaw { get; set; }
    }

    public class PropertyEntryMap:ClassMap<PropertyEntry>
    {
        public PropertyEntryMap()
        {
            Id(x => x.Id);
            Version(x => x.Version);
            Map(x => x.Name);
            References(x => x.EntryClass);
            References(x => x.SchemaClass);
            Map(x => x.SchemaBody);
            Map(x => x.ValString).Nullable();
            Map(x => x.ValInt).Nullable();
            Map(x => x.ValLong).Nullable();
            Map(x => x.ValDateTime).Nullable();
            Map(x => x.ValDouble).Nullable();
            Map(x => x.ValRaw).Nullable();
        }
    }

    public class ClassDescriptor
    {
        public virtual long Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string ClassName { get; set; }
        public virtual string SerializerClassName { get; set; }
        
    }

    public class ClassdescriptorMap:ClassMap<ClassDescriptor>
    {
        public ClassdescriptorMap()
        {
            Id(x => x.Id);
            Map(x => x.Name);
            Map(x => x.ClassName);
            Map(x => x.SerializerClassName);
        }
    }
}
