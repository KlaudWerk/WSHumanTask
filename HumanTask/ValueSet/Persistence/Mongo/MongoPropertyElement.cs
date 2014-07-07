using System;
using HumanTask.ValueSet.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HumanTask.ValueSet.Persistence.Mongo
{
    /// <summary>
    /// Property element that will be stored 
    /// </summary>
    public class MongoPropertyElement:PropertyElement
    {
        private readonly PropertyElement _element;

        /// <summary>
        /// Gets or sets the object identifier.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        [BsonId]
        public ObjectId Id { get; set; }

        public int Version { get; set; }
        #region PropertyElement delegates
        public override string Name
        {
            get { return _element.Name; }
            set { _element.Name = value; }
        }

        public override SerializationValueType SerializationValueType
        {
            get { return _element.SerializationValueType; }
            set { _element.SerializationValueType = value; }
        }

        public override string TypeOfValue
        {
            get { return _element.TypeOfValue; }
            set { _element.TypeOfValue = value; }
        }

        public override string SchemaBody
        {
            get { return _element.SchemaBody; }
            set { _element.SchemaBody = value; }
        }

        public override string SchemaType
        {
            get { return _element.SchemaType; }
            set { _element.SchemaType = value; }
        }

        public override string ValString
        {
            get { return _element.ValString; }
            set { _element.ValString = value; }
        }

        public override int? ValInt
        {
            get { return _element.ValInt; }
            set { _element.ValInt = value; }
        }

        public override long? ValLong
        {
            get { return _element.ValLong; }
            set { _element.ValLong = value; }
        }

        public override DateTime? ValDateTime
        {
            get { return _element.ValDateTime; }
            set { _element.ValDateTime = value; }
        }

        public override double? ValDouble
        {
            get { return _element.ValDouble; }
            set { _element.ValDouble = value; }
        }

        public override byte[] ValRaw
        {
            get { return _element.ValRaw; }
            set { _element.ValRaw = value; }
        }

        #endregion
        /// <summary>
        /// Initializes a new instance of the <see cref="MongoPropertyElement"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public MongoPropertyElement(string name)
        {
            _element = new PropertyElement{Name = name};
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoPropertyElement"/> class.
        /// </summary>
        /// <param name="element">The element.</param>
        public MongoPropertyElement(PropertyElement element)
        {
            _element = element;
        }

    }
}
