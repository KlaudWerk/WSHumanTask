using System.Collections.Generic;
using System.Security.Principal;

namespace HumanTask
{
    /// <summary>
    /// Logical people assignment interface
    /// </summary>
    public interface ILogicalPeopleAssignment
    {
        /// <summary>
        /// Gets the name of the assignment.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; }
        /// <summary>
        /// Gets the Human role.
        /// </summary>
        /// <value>The role.</value>
        HumanRoles Role { get; }
        /// <summary>
        /// Hierarchial list of Members
        /// </summary>
        /// <value>The members list.</value>
        IEnumerable<IIdentity> MembersList { get; }
        /// <summary>
        /// The flat list of members
        /// </summary>
        /// <value>The flat members list.</value>
        IEnumerable<IIdentity> FlatMembersList { get; }
        /// <summary>
        /// Determines whether the specified entity has membership in this assignment.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>
        /// 	<c>true</c> if the specified entity has membership; otherwise, <c>false</c>.
        /// </returns>
        bool HasMembership(IIdentity entity);
    }
}