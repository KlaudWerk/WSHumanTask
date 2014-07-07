using System;

namespace HumanTask.Security
{
    /// <summary>
    /// Type-safe Identity Id
    /// </summary>
    [Serializable]
    public class IdentityId:GuidId<IdentityId>

    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityId"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        public IdentityId(Guid id) : base(id)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityId"/> class.
        /// </summary>
        public IdentityId()
        {
        }
    }
}
