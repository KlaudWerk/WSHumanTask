using System.Collections.Generic;

namespace HumanTask
{
    /// <summary>
    /// The collection of Value Providers
    /// </summary>
    public interface IValueSetCollection:IDictionary<string,object>
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