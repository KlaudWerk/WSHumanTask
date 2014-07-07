
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

namespace Klaudwerk.PropertySet.Mongo
{
    /// <summary>
    /// Extension helper methods 
    /// </summary>
    internal static class PropertyElementExt
    {
        /// <summary>
        /// Fills the bson document.
        /// </summary>
        /// <param name="pval">The pval.</param>
        /// <param name="action">The action.</param>
        public static void FillBsonDocument(this PropertyValue pval, Action<string,BsonValue> action)
        {
            action.Invoke(MongoKeys.SerializationHint,(int)pval.SerializationHint);
            switch (pval.SerializationHint)
            {
                case SerializationTypeHint.Null:
                    action.Invoke(MongoKeys.Value, BsonNull.Value);
                    break;
                case SerializationTypeHint.Bool:
                case SerializationTypeHint.Int:
                    action.Invoke(MongoKeys.Value, pval.Value == null ? BsonNull.Value : (BsonValue)((int?)pval.Value));
                    break;
                case SerializationTypeHint.Long:
                    action.Invoke(MongoKeys.Value, pval.Value == null ? BsonNull.Value : (BsonValue)((long?)pval.Value));
                    break;
                case SerializationTypeHint.Double:
                    action.Invoke(MongoKeys.Value, pval.Value == null ? BsonNull.Value : (BsonValue)((double?)pval.Value));
                    break;
                case SerializationTypeHint.DateTime:
                    action.Invoke(MongoKeys.Value, pval.Value == null ? BsonNull.Value : (BsonValue)((DateTime?)pval.Value));
                    break;
                case SerializationTypeHint.ByteArray:
                case SerializationTypeHint.BinaryObject:
                    if (!string.IsNullOrEmpty(pval.ValueType))
                        action.Invoke(MongoKeys.ValueType, pval.ValueType);
                    action.Invoke(MongoKeys.Value, pval.Value == null ? BsonNull.Value : (BsonValue)((byte[])pval.Value));
                    break;
                case SerializationTypeHint.String:
                case SerializationTypeHint.JsonString:
                case SerializationTypeHint.Object:
                    if(!string.IsNullOrEmpty(pval.ValueType))
                        action.Invoke(MongoKeys.ValueType, pval.ValueType);
                    action.Invoke(MongoKeys.Value, pval.Value == null ? BsonNull.Value : (BsonValue)((string)pval.Value));
                    break;
            }
        }

        /// <summary>
        /// Fills the form bson document.
        /// </summary>
        /// <param name="pe">The Property elemnt to be filled.</param>
        /// <param name="document">The source BSON document.</param>
        public static void FillFormBsonDocument(this PropertyElement pe, BsonDocument document)
        {
            pe.Schema = new SchemaElement();
            pe.Schema.FillSchemaromBsonDocument(document);
            pe.ValueType = document.Contains(MongoKeys.ValueType) ? (document[MongoKeys.ValueType].IsBsonNull ? null : document[MongoKeys.ValueType].AsString) : null;
            pe.FillValueFormBsonDocument(document);
        }

        public static void FillSchemaromBsonDocument(this SchemaElement se,BsonDocument document)
        {
            se.SchemaBody = document.Contains(MongoKeys.SchemaBody)
                                ? (document[MongoKeys.SchemaBody].IsBsonNull
                                       ? null
                                       : document[MongoKeys.SchemaBody].AsString)
                                : null;

            se.SchemaType = document.Contains(MongoKeys.SchemaType)
                                ? (document[MongoKeys.SchemaType].IsBsonNull
                                       ? null
                                       : document[MongoKeys.SchemaType].AsString)
                                : null;
        }

        /// <summary>
        /// Fills the value form bson document.
        /// </summary>
        /// <param name="pe">The source Property Element.</param>
        /// <param name="document">The target BSON document.</param>
        public static void FillValueFormBsonDocument(this PropertyValue pe, BsonDocument document)
        {
            pe.SerializationHint = (SerializationTypeHint)document[MongoKeys.SerializationHint].AsInt32;
            BsonValue element = document[MongoKeys.Value];
            switch (pe.SerializationHint)
            {
                case SerializationTypeHint.Null:
                    pe.Value = null;
                    break;
                case SerializationTypeHint.Bool:
                case SerializationTypeHint.Int:
                    pe.Value = element.IsBsonNull? null: element.AsNullableInt32;
                    break;
                case SerializationTypeHint.Long:
                    pe.Value = element.IsBsonNull ? null : element.AsNullableInt64;
                    break;
                case SerializationTypeHint.Double:
                    pe.Value = element.IsBsonNull? null: element.AsNullableDouble;
                    break;
                case SerializationTypeHint.DateTime:
                    pe.Value = element.IsBsonNull ? null : element.AsNullableDateTime;
                    break;
                case SerializationTypeHint.ByteArray:
                case SerializationTypeHint.BinaryObject:
                    pe.Value = element.IsBsonNull ? null : element.AsByteArray;
                    break;
                case SerializationTypeHint.Object:
                case SerializationTypeHint.String:
                case SerializationTypeHint.JsonString:
                    pe.Value = element.IsBsonNull ? null : element.AsString;
                    break;
            }
            
        }
    }
}