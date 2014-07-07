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
using System.Collections.Generic;
using Klaudwerk.PropertySet.Serialization;

namespace Klaudwerk.PropertySet
{
    /// <summary>
    /// Serialization Capabilities bits
    /// </summary>
    [Flags]
    public enum SerializationCapabilities
    {
        /// <summary>
        /// The serializer supports Binary Serialization
        /// </summary>
        Binary,
        /// <summary>
        /// The serializer supports JSON serialization
        /// </summary>
        Json,
        /// <summary>
        /// Serialization of primitive values
        /// </summary>
        PrimitiveValue
    }

    public interface IValueSerializationTarget
    {
        void Set(int? value);
        void Set(long? value);
        void Set(double? value);
        void Set(bool? value);
        void Set(string value);
        void Set(DateTime? value);
        void Set(byte[] value);
        void Set(byte[] value, Type type);
        void Set(string value, Type type);
    }
    /// <summary>
    /// The Value Serializer interface
    /// </summary>
    public interface IValueSerializer<T>
    {
        /// <summary>
        /// Gets the capabilities.
        /// </summary>
        /// <value>The capabilities.</value>
        SerializationCapabilities Capabilities { get; }
        /// <summary>
        /// Serializing of a primitive type
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="target">The call back.</param>
        void Serialize(T value, IValueSerializationTarget target);
        /// <summary>
        /// Serialize an object to Binary serialization format
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        byte[] ToBinary(T value);
        /// <summary>
        /// Serialize an object 
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        string ToJson(T value);
        /// <summary>
        /// Deserializes the specified json string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonString">The json string.</param>
        /// <returns></returns>
        T Deserialize(string jsonString);
        /// <summary>
        /// Deserializes the specified binary data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="binaryData">The binary data.</param>
        /// <returns></returns>
        T Deserialize(byte[] binaryData);

    }
}