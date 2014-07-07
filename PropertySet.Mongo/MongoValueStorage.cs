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
using Klaudwerk.PropertySet.Serialization;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace Klaudwerk.PropertySet.Mongo
{
    /// <summary>
    /// Private Mongo Collection "Storage" class
    /// </summary>
    internal class MongoValueStorage : MongoCollectionDictionary<string, object>
    {
        private readonly IPropertySchemaSet _schemas;
        private readonly MongoCollection<BsonDocument> _storage;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoValueStorage"/> class.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="schemas">The schemas.</param>
        public MongoValueStorage(
            MongoCollection<BsonDocument> collection,
            IPropertySchemaSet schemas)
            : base(collection)
        {
            _schemas = schemas;
            _storage = collection;
        }

        /// <summary>
        /// Adds the with schema.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <param name="schema">The schema.</param>
        internal void AddWithSchema(string name, object value, IValueSchema<object> schema)
        {
            try
            {
                BsonDocument document = new BsonDocument
                                            {
                                                {MongoKeys.KeyField, name},
                                            };
                if (schema != null)
                {
                    string sbody;
                    Type stype;
                    _schemas.SchemaFactory.SerializeSchema(schema, out stype, out sbody);
                    document.Add(MongoKeys.SchemaType, stype.FullName);
                    document.Add(MongoKeys.SchemaBody, sbody);
                }
                else
                {
                    schema = value == null ? null : _schemas.SchemaFactory.Create(value.GetType());
                }
                MongoPropertyValue pv =new MongoPropertyValue();
                if(value!=null)
                {
                    schema.Serializer.Serialize(value,pv);
                }
                else
                {
                    pv.SerializationHint = SerializationTypeHint.Null;
                    pv.Value = null;
                }
                pv.FillBsonDocument((k, v) => document.Add(k, v));
                _storage.Insert(document, SafeMode.True);
            }
            catch (MongoSafeModeException ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }

        /// <summary>
        /// Adds the value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        internal void AddValue(string name, object value)
        {
            BsonDocument document = new BsonDocument
                                        {
                                            {MongoKeys.KeyField, name}
                                        };
            if (value == null)
            {
                document.Add(new BsonElement(MongoKeys.SerializationHint, (int)SerializationTypeHint.Object));
                document.Add(new BsonElement(MongoKeys.Value, BsonNull.Value));
            }
            else
            {
                IValueSchema<object> schema = _schemas.SchemaFactory.Create(value.GetType());
                MongoPropertyValue pv = new MongoPropertyValue();
                schema.Serializer.Serialize(value,pv);
                pv.FillBsonDocument((k, v) => document.Add(k, v));
            }
            try
            {
                _storage.Insert(document, SafeMode.True);
            }
            catch (MongoSafeModeException ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }

        public void StoreValue(string key,object value)
        {
            ValidateValue(key,value);
            this[key] = value;
        }

        private void ValidateValue(string key, object value)
        {
            IValueSchema<object> schema;
            if (_schemas.TryGetSchema(key, out schema))
            {
                schema.Validate(value);
            }
        }

        /// <summary>
        /// Converts to collection element value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        protected override BsonDocument ConvertToCollectionElementValue(string key, object value)
        {
            PropertyValue pval = SerializeValue(key, value);
            BsonDocument document = new BsonDocument
                                        {
                                            {GetKeyField(), key}
                                        };
            pval.FillBsonDocument((k, v) => document.Add(k, v));
            return document;
        }
        /// <summary>
        /// Toes the bson value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        protected override BsonValue ToBsonValue(string key)
        {
            return new BsonString(key);
        }

        /// <summary>
        /// Gets the name of the key field.
        /// </summary>
        /// <returns></returns>
        protected override string GetKeyField()
        {
            return MongoKeys.KeyField;
        }

        /// <summary>
        /// Gets the key field value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        protected override string GetKeyFieldValue(BsonDocument value)
        {
            return value[GetKeyField()].AsString;
        }

        /// <summary>
        /// Gets the updates.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        protected override UpdateBuilder GetUpdates(string key, object value)
        {
            PropertyValue pval = SerializeValue(key, value);
            UpdateBuilder builder = new UpdateBuilder();
            pval.FillBsonDocument((k, v) => builder.Set(k, v));
            return builder;
        }

        /// <summary>
        /// Converts the value.
        /// </summary>
        /// <param name="mongoElement">The mongo element.</param>
        /// <returns></returns>
        protected override object ConvertValue(BsonDocument mongoElement)
        {
            PropertyElement pe = new PropertyElement();
            pe.FillFormBsonDocument(mongoElement);
            if(pe.SerializationHint!=SerializationTypeHint.JsonString && 
                pe.SerializationHint!=SerializationTypeHint.BinaryObject)
            {
                return pe.Value;
            }
            string name = mongoElement[GetKeyField()].AsString;
            IValueSchema<object> schema;
            if(!_schemas.TryGetSchema(name, out schema))
            {
                schema = _schemas.SchemaFactory.Create(pe.ValueType);
            }
            if(pe.SerializationHint==SerializationTypeHint.JsonString)
            {
                return schema.Serializer.Deserialize((string) pe.Value);
            }
            if (pe.SerializationHint == SerializationTypeHint.BinaryObject)
            {
                return schema.Serializer.Deserialize((byte[])pe.Value);
            }
            throw new ArgumentException("Unsupported serialization");
        }

        /// <summary>
        /// Tries the convert value.
        /// </summary>
        /// <param name="mongoElement">The mongo element.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        protected override bool TryConvertValue(BsonDocument mongoElement, out object value)
        {
            value = ConvertValue(mongoElement);
            return value != null;
        }

        private PropertyValue SerializeValue(string key,object value)
        {
            IValueSchema<object> schema;
            MongoPropertyValue pval = new MongoPropertyValue();
            if (_schemas.TryGetSchema(key, out schema))
            {
                schema.Serializer.Serialize(value, pval);
            }
            else
            {
                schema = value == null ? null : _schemas.SchemaFactory.Create(value.GetType());
                if (schema != null)
                    schema.Serializer.Serialize(value, pval);

            }
            return pval;
        }

        private class MongoPropertyValue:PropertyValue,IValueSerializationTarget
        {
            public void Set(int? value)
            {
                ValueType = (typeof (int?)).FullName;
                Value = value;
                SerializationHint = SerializationTypeHint.Int;
            }

            public void Set(long? value)
            {
                ValueType = (typeof(long?)).FullName;
                Value = value;
                SerializationHint = SerializationTypeHint.Long;
            }

            public void Set(double? value)
            {
                ValueType = (typeof(double?)).FullName;
                Value = value;
                SerializationHint = SerializationTypeHint.Double;
            }

            public void Set(bool? value)
            {
                ValueType = (typeof(bool?)).FullName;
                Value = (value.HasValue && value.Value)?1:0;
                SerializationHint = SerializationTypeHint.Bool;
            }

            public void Set(string value)
            {
                ValueType = (typeof(string)).FullName;
                Value = value;
                SerializationHint = SerializationTypeHint.String;
            }

            public void Set(DateTime? value)
            {
                ValueType = (typeof(DateTime?)).FullName;
                Value = value;
                SerializationHint = SerializationTypeHint.DateTime;
            }

            public void Set(byte[] value)
            {
                ValueType = (typeof(byte[])).FullName;
                Value = value;
                SerializationHint = SerializationTypeHint.ByteArray;
            }

            public void Set(byte[] value, Type type)
            {
                ValueType = type.FullName;
                Value = value;
                SerializationHint = SerializationTypeHint.BinaryObject;
            }

            public void Set(string value, Type type)
            {
                ValueType = type.FullName;
                Value = value;
                SerializationHint = SerializationTypeHint.JsonString;
            }
        }
    }
}