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
namespace Klaudwerk.PropertySet.Serialization
{
    /// <summary>
    /// Enum that define "quick" serialization data types
    /// </summary>
    public enum SerializationTypeHint
    {
        /// <summary>
        /// The type is string
        /// </summary>
        String =1,
        /// <summary>
        /// The type is integer
        /// </summary>
        Int,
        /// <summary>
        /// The type ils Long
        /// </summary>
        Long,
        /// <summary>
        /// The type is Bool
        /// </summary>
        Bool,
        /// <summary>
        /// The type is Double
        /// </summary>
        Double,
        /// <summary>
        /// The type is Date Time
        /// </summary>
        DateTime,
        /// <summary>
        /// The type is a Byte array
        /// </summary>
        ByteArray,
        /// <summary>
        /// The type is Object
        /// </summary>
        Object,
        /// <summary>
        /// The value is Null
        /// </summary>
        Null,
        /// <summary>
        /// Json String
        /// </summary>
        JsonString,
        /// <summary>
        /// Binary Serialized object
        /// </summary>
        BinaryObject
    }
}