using DataServiceDesigner.Domain;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace DataServiceDesigner.DataService
{
    public class DomainObjectReferenceMap : ClassMapping<DomainObjectReference>
    {
        public DomainObjectReferenceMap()
        {
            Table("domainobjectreference");

            Id(x => x.Id);
            
            ManyToOne(x => x.Schema, m =>
            {
                m.Column("schemaid");
                m.NotNullable(true);
                m.Cascade(Cascade.Persist);
                m.Fetch(FetchKind.Join);
                m.Lazy(LazyRelation.NoLazy);
            });

            ManyToOne(x => x.Parent, m =>
            {
                m.Column("parentobjectid");
                m.NotNullable(true);
                m.Cascade(Cascade.Persist);
                m.Fetch(FetchKind.Join);
                m.Lazy(LazyRelation.NoLazy);
            });

            ManyToOne(x => x.Child, m =>
            {
                m.Column("childobjectid");
                m.NotNullable(true);
                m.Cascade(Cascade.Persist);
                m.Fetch(FetchKind.Join);
                m.Lazy(LazyRelation.NoLazy);
            });

            Property(x => x.ReferenceName, m => m.NotNullable(true));
            Property(x => x.ConstraintName, m => m.NotNullable(true));
            Property(x => x.ReferenceType, m => m.NotNullable(true));

            Bag(x => x.Properties,
                m =>
                {
                    m.Key(k => k.Column("referenceid"));
                    m.Cascade(Cascade.All.Include(Cascade.DeleteOrphans));
                    m.Lazy(CollectionLazy.NoLazy);
                    m.Inverse(true);
                },
                r => r.OneToMany());
        }
    }
}
