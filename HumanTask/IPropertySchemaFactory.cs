using System;

namespace HumanTask
{
    /// <summary>
    /// Schema Factory interface
    /// </summary>
    public interface IPropertySchemaFactory
    {
        /// <summary>
        /// Creates new schema for a property of type t
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns></returns>
        IValueSchema<object> Create(Type t);
        /// <summary>
        /// Registers the schema for this factory.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <param name="schema">The schema.</param>
        void RegisterSchema(Type t, Func<IValueSchema<object>> schema);
    }
}