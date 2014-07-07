using System;
using System.Collections.Generic;
using System.Security.Principal;

namespace HumanTask.Security
{
    public class AssignmentServiceImpl:IAssignmentService
    {
        #region Implementation of IAssignmentService

        private readonly TaskEntity _entity;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssignmentServiceImpl"/> class.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public AssignmentServiceImpl(TaskEntity entity)
        {
            _entity = entity;
        }

        /// <summary>
        /// Assigns the specified role.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <param name="identities">The identities.</param>
        public void Assign(HumanRoles role, IEnumerable<IdentityId> identities)
        {
            RoleAssignmentEntity assignment = GetAssignment(role);
            foreach (IdentityId identity in identities)
            {
                if(!assignment.Identities.Contains(identity))
                    assignment.Identities.Add(identity);
                if (assignment.ExcludedIdentities.Contains(identity))
                    assignment.ExcludedIdentities.Remove(identity);
            }
        }

        /// <summary>
        /// Assigns the specified role.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <param name="expression">The expression.</param>
        public void Assign(HumanRoles role, string expression)
        {
            RoleAssignmentEntity assignment = GetAssignment(role);
            assignment.IdentitiesSelectExpression = expression;
        }

        /// <summary>
        /// Excludes specified identities from the asssignment.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <param name="identities">The identities.</param>
        public void Exclude(HumanRoles role, IEnumerable<IdentityId> identities)
        {
            RoleAssignmentEntity assignment = GetAssignment(role);
            foreach (IdentityId identity in identities)
            {
                if (assignment.Identities.Contains(identity))
                    assignment.Identities.Remove(identity);
                if (!assignment.ExcludedIdentities.Contains(identity))
                    assignment.ExcludedIdentities.Add(identity);
            }
        }

        /// <summary>
        /// Removes the assignment.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public void RemoveAssignment(HumanRoles role, string expression)
        {
            Assign(role,string.Empty);
        }

        /// <summary>
        /// Removes the assignment.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <param name="identities">The identities.</param>
        /// <returns></returns>
        public void RemoveAssignment(HumanRoles role, IEnumerable<IdentityId> identities)
        {
            RoleAssignmentEntity assignment = GetAssignment(role);
            foreach (IdentityId identity in identities)
            {
                if (assignment.Identities.Contains(identity))
                    assignment.Identities.Remove(identity);
                if (assignment.ExcludedIdentities.Contains(identity))
                    assignment.ExcludedIdentities.Remove(identity);
            }
        }

        /// <summary>
        /// Gets the assignments for a Human role.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <returns></returns>
        public IEnumerable<IIdentity> GetAssignments(HumanRoles role)
        {
            throw new NotImplementedException();
        }
        #endregion

        private RoleAssignmentEntity GetAssignment(HumanRoles role)
        {
            RoleAssignmentEntity roleAssignment;
            if (!_entity.RoleAssignments.TryGetValue(role, out roleAssignment))
            {
                roleAssignment = new RoleAssignmentEntity();
                _entity.RoleAssignments[role] = roleAssignment;
            }
            return roleAssignment;
        }
    }
}
