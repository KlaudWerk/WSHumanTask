/**
The MIT License (MIT)

Copyright (c) 2013 Igor Polouektov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
  */
using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Type;

namespace HumanTask.Hibernate
{
    /// <summary>
    /// Mapping for Task
    /// </summary>
    public class TaskEntityMap:ClassMapping<TaskEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TaskEntityMap"/> class.
        /// </summary>
        public TaskEntityMap()
        {
            Table("Task");
            Id(x => x.Id, map =>
            {
                map.Generator(Generators.Identity);
                map.Column("task_persistent_id");
            });

            Version(x => x.Version, m =>
            {
                m.Column("version");
                m.UnsavedValue(0);
                m.Insert(true);
                m.Type(new Int32Type());
                m.Generated(VersionGeneration.Never);
            });

            #region Properties Mapping

            Property(x => x.TaskId, m =>
                                        {
                                            m.Column("task_id");
                                            m.NotNullable(true);
                                            m.UniqueKey("k_task_id");
                                            m.Index("idx_task_entity_id");
                                            m.Type(typeof(TaskIdType),null);
                                        });
            Property(x=>x.Name,m=>m.Column("name"));
            Property(x => x.Subject, map =>
                                         {
                                             map.Column("subject");
                                             map.Length(4096);
                                         });
            Property(x => x.IsSkippable, m => m.Column("skippable"));
            Property(x => x.Created, m =>
                                         {
                                             m.NotNullable(true); 
                                             m.Column("date_created");
                                         });
            Property(x => x.Started, m =>
                                         {
                                             m.NotNullable(false); 
                                             m.Column("date_started");
                                         });
            Property(x => x.Completed, m =>
                                           {
                                               m.NotNullable(false); 
                                               m.Column("date_completed");
                                           });
            Property(x => x.Status, m =>
                                        {
                                            m.Column("status");
                                            m.NotNullable(true);
                                        });
            Property(x => x.Priority, m =>
                                          {
                                              m.Column("priority");
                                              m.NotNullable(true);
                                          });
            Property(x => x.Initiator, m =>
                                           {
                                               m.Column("initiator");
                                               m.NotNullable(true);
                                               m.Type(typeof (IdentityType), null);
                                           });

            Property(x => x.ActualOwner, m =>
                                            {
                                                m.Column("owner");
                                                m.Type(typeof(IdentityType), null);
                                            });

            Property(x=>x.TaskOutcome,m=>m.Column("task_output"));
            // ---------------------------------
            // Suspemded state component mapping
            // ---------------------------------
            Component(x=>x.SuspendedState,
                m=>
                    {
                        m.Property(p=>p.OperationPerformed,map=>map.Column("operation_date"));
                        m.Property(p=>p.SuspensionEnds,map=>map.Column("suspension_end_date"));
                        m.Property(p=>p.OriginalStatus,map=>map.Column("original_status"));
                        m.Property(p => p.OriginalOwner, map =>
                                                             {
                                                                 map.Column("original_owner");
                                                                 map.Type(typeof(IdentityType),null);
                                                             });
                    });
            #endregion

            #region Task Comments Mapping
            //-------------------------------------
            // The Task Comments Collection mapping
            //-------------------------------------
            Set(x => x.Comments, set =>
            {
                set.Key(k =>
                {
                    k.Column("task_id");
                    k.ForeignKey("FK_Task_TaskComment");
                });
                set.Table("Task_Comments");

                set.Cascade(Cascade.All.Include(Cascade.Remove));
                set.Fetch(CollectionFetchMode.Subselect);
                set.OrderBy(c => c.TimeStamp);
            },
                                     mapping => mapping.Component(c =>
                                     {
                                         c.Property(p => p.Comment, m =>
                                         {
                                             m.Column("text");
                                             m.Type(NHibernateUtil.StringClob);
                                         });
                                         c.Property(p => p.TimeStamp, m => m.Column("ts"));
                                         c.Property(p => p.UserId, m =>
                                         {
                                             m.Column("user_id");
                                             m.Type(typeof(IdentityType), null);
                                         });
                                     }));
            #endregion

            #region Subtasks Mapping
            // ---------------------------
            // Subtasks Collection Mapping
            // ---------------------------
            Bag(x => x.Subtasks, map =>
            {
                map.BatchSize(100);
                map.Lazy(CollectionLazy.Lazy);
                map.Cascade(Cascade.All);
                map.Key(k => k.Column("task_parent_id"));
                map.Inverse(true);

            },
                                     c => c.OneToMany());
            // -----------------------
            // The Task Parent mapping
            // -----------------------
            ManyToOne(x => x.Parent, manyToOne =>
            {
                manyToOne.Column("task_parent_id");
                manyToOne.NotNullable(false);
                manyToOne.Lazy(LazyRelation.NoLazy);
            });
            #endregion

            #region Role Assignments Collections Mapping 

            Set(x=>x.PotentialOwners,set=>
                                         {
                                             set.Key(k =>
                                             {
                                                 k.Column("task_id");
                                                 k.ForeignKey("FK_Task_PotentialOwners");
                                             });
                                             set.Table("Potential_Owners");
                                             set.Cascade(Cascade.All.Include(Cascade.Remove));
                                             set.Fetch(CollectionFetchMode.Subselect);
                                         },
                                         map => map.Element(e =>
                                                                {
                                                                    e.Type(typeof (IdentityType), null);
                                                                    e.Column("mapped_identity_id");
                                                                }));

            Set(x => x.ExcludedOwners, set =>
            {
                set.Key(k =>
                {
                    k.Column("task_id");
                    k.ForeignKey("FK_Task_ExcludedOwners");
                });
                set.Table("Excluded_Owners");
                set.Cascade(Cascade.All.Include(Cascade.Remove));
                set.Fetch(CollectionFetchMode.Subselect);
            },
            map => map.Element(e =>
                                   { 
                                       e.Type(typeof (IdentityType), null);
                                       e.Column("mapped_identity_id");
                                   }));

            Set(x => x.BusinessAdministrators, set =>
            {
                set.Key(k =>
                {
                    k.Column("task_id");
                    k.ForeignKey("FK_Task_BusinessAdministrators");
                });
                set.Table("Business_Administrators");
                set.Cascade(Cascade.All.Include(Cascade.Remove));
                set.Fetch(CollectionFetchMode.Subselect);
            },
            map => map.Element(e =>
                                   { 
                                       e.Type(typeof (IdentityType), null);
                                       e.Column("mapped_identity_id");
                                   }));

            Set(x => x.PotentialDelegatees, set =>
            {
                set.Key(k =>
                {
                    k.Column("task_id");
                    k.ForeignKey("FK_Task_PotentialDelegatees");
                });
                set.Table("Potential_Delegatees");
                set.Cascade(Cascade.All.Include(Cascade.Remove));
                set.Fetch(CollectionFetchMode.Subselect);
            },
            map => map.Element(e => {
                                        e.Type(typeof (IdentityType), null);
                                        e.Column("mapped_identity_id");
            }));

            Set(x => x.Recepients, set =>
            {
                set.Key(k =>
                {
                    k.Column("task_id");
                    k.ForeignKey("FK_Task_Recepients");
                });
                set.Table("Recepients");
                set.Cascade(Cascade.All.Include(Cascade.Remove));
                set.Fetch(CollectionFetchMode.Subselect);
            },
            map => map.Element(e =>
                                   {
                                       e.Type(typeof (IdentityType), null);
                                       e.Column("mapped_identity_id");
                                   }));

            Set(x => x.Stakeholders, set =>
            {
                set.Key(k =>
                {
                    k.Column("task_id");
                    k.ForeignKey("FK_Task_Stakeholders");
                });
                set.Table("Stakeholders");
                set.Cascade(Cascade.All.Include(Cascade.Remove));
                set.Fetch(CollectionFetchMode.Subselect);
            },
            map => map.Element(e =>
                                   {
                                       e.Type(typeof (IdentityType), null);
                                       e.Column("mapped_identity_id");
                                   }));

            #endregion
        }
    }
}
