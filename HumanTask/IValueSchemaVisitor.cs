namespace HumanTask
{
    /// <summary>
    /// Value Schema Visitor
    /// </summary>
    public interface IValueSchemaVisitor
    {
        /// <summary>
        /// Visits the specified 'reference-type' schema.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="schema">The schema.</param>
        void Visit<T>(IValueSchema<T> schema) where T : class;
        /// <summary>
        /// Visits the specified 'nullable value type' schema.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="schema">The schema.</param>
        void Visit<T>(IValueSchema<T?> schema) where T : struct ;
    }
}