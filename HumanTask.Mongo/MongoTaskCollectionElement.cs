using System;
using System.Collections.Generic;

namespace HumanTask.Mongo
{
    /// <summary>
    /// Mongo collection element that represent a task
    /// </summary>
    public class MongoTaskCollectionElement
    {
        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>The version.</value>
        public int Version { get; set; }
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        public string Id { get; set; }
        /// <summary>
        /// Gets or sets the task parent id.
        /// </summary>
        /// <value>The parent id.</value>
        public string  ParentId { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        /// <value>The subject.</value>
        public string Subject { get; set; }
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>The status.</value>
        public int Status { get; set; }
        /// <summary>
        /// Gets or sets the string representation of a task priority.
        /// </summary>
        /// <value>The priority.</value>
        public int Priority { get; set; }

        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>The created.</value>
        public DateTime Created { get; set; }
        /// <summary>
        /// Gets or sets the started.
        /// </summary>
        /// <value>The started.</value>
        public DateTime Started { get; set; }
        /// <summary>
        /// Gets or sets the completed date time.
        /// DateTime.MinValue indicates that the task was not completed
        /// </summary>
        /// <value>The completed.</value>
        public DateTime Completed { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="MongoTaskCollectionElement"/> is skippable.
        /// </summary>
        /// <value><c>true</c> if skippable; otherwise, <c>false</c>.</value>
        public  bool Skippable { get; set; }

        /// <summary>
        /// Gets or sets the initiator id.
        /// </summary>
        /// <value>The initiator id.</value>
        public Guid InitiatorId { get; set; }
        /// <summary>
        /// Gets or sets the actual owner id.
        /// </summary>
        /// <value>The actual owner id.</value>
        public Guid? ActualOwnerId { get; set; }

        public string Outcome { get; set; }

        public List<TaskComment> Comments { get; set; }
    }
}
