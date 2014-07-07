using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace HumanTask.Hibernate
{
    /// <summary>
    /// Mapping for Task
    /// </summary>
    public class TaskMap:ClassMapping<ITask>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TaskMap"/> class.
        /// </summary>
        public TaskMap()
        {
            Table("Task");
            Id(x => x.Id, map =>
            {
                map.Generator(Generators.Assigned);
                map.Column("task_id");
            });
            Property(x=>x.Name);
            Property(x=>x.Subject);
            Property(x => x.IsSkippable, m => m.Column("skippable"));
            Property(x => x.Created,m=>m.NotNullable(true));
            Property(x => x.Started, m => m.NotNullable(false));
            Property(x => x.Completed, m => m.NotNullable(false));
            Property(x=>x.Status);
            Property(x => x.Priority);
        }
    }
}
