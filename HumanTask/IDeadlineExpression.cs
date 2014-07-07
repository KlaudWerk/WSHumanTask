namespace HumanTask
{
    /// <summary>
    /// The Deadline expression.
    /// </summary>
    public interface IDeadlineExpression
    {
        /// <summary>
        /// Evaluates the deadline expression. 
        /// </summary>
        /// <returns>true is this deadline has been met; false otherwise</returns>
        bool Evaluate();
    }
}