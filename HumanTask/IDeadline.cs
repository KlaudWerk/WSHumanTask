using System.Collections.Generic;

namespace HumanTask
{
    public interface IDeadline
    {
        string Name { get; }
        IDeadlineExpression Expression { get; }
        IEscalation Escalation { get; }
        ICollection<INotificationDefinition> Notifications { get; }
    }
}