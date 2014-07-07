using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace HumanTask.Hibernate
{
    /// <summary>
    /// NHibernate mapping for a Role assignment
    /// </summary>
    public class RoleAssignmentEntityMap:ClassMapping<RoleAssignmentEntity>
    {
        public RoleAssignmentEntityMap()
        {
            Table("Role_Assignment");
            Id(x => x.Id, map =>
                              {
                                  map.Generator(Generators.Identity);
                                  map.Column("role_assignment_persistent_id");
                              });
            Property(x => x.IdentitiesSelectExpression, m =>
                                                            {
                                                                m.Column("expression");
                                                                m.NotNullable(false);
                                                            });
            Set(x => x.Identities, set =>
                                       {
                                           set.Table("Role_Assignment_Included");
                                           set.Key(k =>
                                                       {
                                                           k.Column("role_assignment_persistent_id");
                                                           k.ForeignKey("FK_role_assignment_persistent_id_inc");
                                                       });
                                           set.Cascade(Cascade.All.Include(Cascade.Remove));
                                           set.Fetch(CollectionFetchMode.Subselect);
                                       },
                map => map.Element(e =>
                                       {
                                           e.Type(typeof (IdentityIdType), null);
                                           e.Column("identity_id");
                                       }));
            Set(x => x.ExcludedIdentities, set =>
                                               {
                                                   set.Table("Role_Assignment_Excluded");
                                                   set.Key(k =>
                                                               {
                                                                   k.Column("role_assignment_persistent_id");
                                                                   k.ForeignKey("FK_role_assignment_persistent_id_exc");
                                                               });
                                                   set.Cascade(Cascade.All.Include(Cascade.Remove));
                                                   set.Fetch(CollectionFetchMode.Subselect);
                                               },
                map => map.Element(e =>
                                       {
                                           e.Type(typeof(IdentityIdType), null);
                                           e.Column("identity_id");
                                       }));

        }
    }
}