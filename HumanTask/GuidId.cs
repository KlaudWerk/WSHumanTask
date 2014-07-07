using System;

namespace HumanTask
{
    /// <summary>
    /// Type-safe Guid-base ID
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class GuidId<T> : IEquatable<GuidId<T>> where T:GuidId<T>
    {
        private Guid _id;

        /// <summary>
        /// Gets the id.
        /// </summary>
        /// <value>The id.</value>
        public Guid Id
        {
            get { return _id; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GuidId&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        public GuidId(Guid id)
        {
            _id = id;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GuidId&lt;T&gt;"/> class.
        /// </summary>
        public GuidId()
        {
            _id = Guid.NewGuid();
        }

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        public bool Equals(GuidId<T> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other._id.Equals(_id);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (GuidId<T>)) return false;
            return Equals((GuidId<T>) obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(GuidId<T> left, GuidId<T> right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(GuidId<T> left, GuidId<T> right)
        {
            return !Equals(left, right);
        }
    }
}