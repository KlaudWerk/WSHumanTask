namespace HumanTask
{
    /// <summary>
    /// Schema Visitable interface
    /// </summary>
    public interface ISchemaVisitable
    {
        /// <summary>
        /// Accepts the schema visitor.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        void Accept(IValueSchemaVisitor visitor);
    }
}