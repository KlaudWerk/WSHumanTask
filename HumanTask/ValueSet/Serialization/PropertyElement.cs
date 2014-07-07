using System;
using System.Runtime.Serialization;

namespace HumanTask.ValueSet.Serialization
{
    /// <summary>
    /// Serialized Property Entry
    /// </summary>
    [Serializable]
    [DataContract]
    public class PropertyElement
    {

        [DataMember]
        public virtual string Name { get; set; }
        /// <summary>
        /// Gets or sets the type of the serialization value.
        /// </summary>
        /// <value>The type of the serialization value.</value>
        [DataMember]
        public virtual SerializationValueType SerializationValueType
        {
            get; set;
        }
        [DataMember]
        public virtual string TypeOfValue
        {
            get;set;
        }
        [DataMember]
        public virtual string SchemaBody
        {
            get;set;
        }
        [DataMember]
        public virtual string SchemaType
        {
            get;set;
        }
        [DataMember]
        public virtual string ValString
        {
            get;set;
        }
        [DataMember]
        public virtual int? ValInt
        {
            get;set;
        }
        [DataMember]
        public virtual long? ValLong
        {
            get; set;
        }
        [DataMember]
        public virtual DateTime? ValDateTime
        {
            get; set;
        }
        [DataMember]
        public virtual double? ValDouble
        {
            get; set;
        }
        [DataMember]
        public virtual byte[] ValRaw
        {
            get; set;
        }
    }
}