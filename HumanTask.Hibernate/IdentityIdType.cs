using System;
using HumanTask.Security;

namespace HumanTask.Hibernate
{
    /// <summary>
    /// Custom User Type for IdentityId
    /// </summary>
    public class IdentityIdType:GuidIdType<IdentityId>
    {
        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="val">The val.</param>
        /// <returns></returns>
        protected override Guid? GetValue(object val)
        {
            if (val == null)
                return null;
            IdentityId iId = val as IdentityId;
            if (iId == null)
                throw new ArgumentException(string.Format("Invalid type: {0}", val.GetType().FullName));
            return iId.Id;
        }

        /// <summary>
        /// Insts the value.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <returns></returns>
        protected override IdentityId InstValue(object o)
        {
            return o==null?null:new IdentityId((Guid)o);
        }

    }
}