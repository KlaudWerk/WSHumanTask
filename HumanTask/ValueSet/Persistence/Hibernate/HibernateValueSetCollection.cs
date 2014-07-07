using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HumanTask.ValueSet.Serialization;

namespace HumanTask.ValueSet.Persistence.Hibernate
{

    /// <summary>
    /// Value Set collection with NHibernate persistence
    /// </summary>
    public class HibernateValueSetCollection:ValueSetCollectionBase
    {
        private PropertyCollection GetCollection()
        {
            throw new NotImplementedException();
        }
        
        public HibernateValueSetCollection(IPropertySchemaFactory schemaFactory)
        {
        }

        protected override IDictionary<string, object> Storage
        {
            get { throw new NotImplementedException(); }
        }

        protected override void Store(string name, object value)
        {
            
        }

        protected override void AddWithSchema(string name, object value, IValueSchema<object> schema)
        {
            PropertyElement element = new PropertyElement
                                      {
                                               
                                       };

            GetCollection().Values.Add(name,element);

        }
        /// <summary>
        /// Gets the Properties Schema Set.
        /// </summary>
        /// <value>The schemas.</value>
        public override IPropertySchemaSet Schemas
        {
            get { throw new NotImplementedException(); }
        }

        private class ProertySchemaSet:PropertySchemaSetBase
        {
            public ProertySchemaSet(IPropertySchemaFactory schemaFactory) : base(schemaFactory)
            {
            }

            protected override IDictionary<string, IValueSchema<object>> SchemaStore
            {
                get { throw new NotImplementedException(); }
            }
        }
    }
}
