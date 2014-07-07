using System.Collections.Generic;

namespace HumanTask.ValueSet.Serialization
{
    /// <summary>
    /// Property Collection class
    /// </summary>
    public class PropertyCollection
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        public virtual long Id { get; private set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public virtual string Name { get; set; }
        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>The version.</value>
        public virtual int Version { get; set; }
        /// <summary>
        /// Gets or sets the values.
        /// </summary>
        /// <value>The values.</value>
        public virtual IDictionary<string, PropertyElement> Values { get; private set; }
    }
}
