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
using System.Collections.Generic;

namespace Klaudwerk.PropertySet
{
    /// <summary>
    /// The collection of Value Providers
    /// </summary>
    public interface IPropertySetCollection:IDictionary<string,object>
    {
        /// <summary>
        /// Gets the Properties Schema Set.
        /// </summary>
        /// <value>The schemas.</value>
        IPropertySchemaSet Schemas { get; }
        /// <summary>
        /// Adds the specified name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <param name="schema">The schema.</param>
        /// <returns></returns>
        T? Add<T>(string name, IValueSchema<T?> schema) where T : struct;
        /// <summary>
        /// Adds the specified name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <param name="schema">The schema.</param>
        /// <returns></returns>
        T? Add<T>(string name, T? value, IValueSchema<T?> schema) where T : struct;
        /// <summary>
        /// Adds the specified name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <param name="schema">The schema.</param>
        /// <returns></returns>
        T Add<T>(string name, IValueSchema<T> schema) where T : class;
        /// <summary>
        /// Adds the specified name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <param name="schema">The schema.</param>
        /// <returns></returns>
        T Add<T>(string name, T value, IValueSchema<T> schema) where T : class;
        /// <summary>
        /// Adds the specified name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        void Set<T>(string name, T value) where T : class;
        /// <summary>
        /// Sets the specified name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        void Set<T>(string name, T? value) where T : struct;
        /// <summary>
        /// Gets the value of the specified property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <returns>the value</returns>
        T Get<T>(string name);
        /// <summary>
        /// Runs the validation on the collection
        /// </summary>
        void Validate();

    }
}