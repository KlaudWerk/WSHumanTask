using System.Security.Principal;

namespace HumanTask.Security
{
    /// <summary>
    /// External identity surrogate
    /// </summary>
    public class IdentitySurrogate:IIdentity 
    {
        public IdentityId Id { get; private set; }
        public string ExternalId { get; private set; }
        public bool IsGroup { get; private set; }
        public bool IsRole { get; private set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="IdentitySurrogate"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="externalId">The external id.</param>
        /// <param name="name">The name.</param>
        /// <param name="authenticationType">Type of the authentication.</param>
        /// <param name="isGroup">if set to <c>true</c> [is group].</param>
        /// <param name="isRole">if set to <c>true</c> [is role].</param>
        public IdentitySurrogate(IdentityId id, 
            string externalId, 
            string name, 
            string authenticationType,
            bool isGroup,
            bool isRole
            )
        {
            Id = id;
            ExternalId = externalId;
            Name = name;
            AuthenticationType = authenticationType;
            IsRole = isRole;
            IsGroup = isGroup;
        }

        /// <summary>
        /// Gets the name of the current user.
        /// </summary>
        /// <returns>
        /// The name of the user on whose behalf the code is running.
        /// </returns>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the type of authentication used.
        /// </summary>
        /// <returns>
        /// The type of authentication used to identify the user.
        /// </returns>
        public string AuthenticationType { get; private set; }

        /// <summary>
        /// Gets a value that indicates whether the user has been authenticated.
        /// </summary>
        /// <returns>
        /// true if the user was authenticated; otherwise, false.
        /// </returns>
        public bool IsAuthenticated { get; set; }
    }
}
