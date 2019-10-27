using DataServiceDesigner.Domain;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace DataServiceDesigner.DataService
{
    public class DomainObjectReferencePropertyMap : ClassMapping<DomainObjectReferenceProperty>
    {
        public DomainObjectReferencePropertyMap()
        {
            Table("domainobjectreferenceproperty");

            Id(x => x.Id);

            ManyToOne(x => x.Reference, m =>
            {
                m.Column("referenceid");
                m.NotNullable(true);
                m.Cascade(Cascade.Persist);
                m.Fetch(FetchKind.Join);
                m.Lazy(LazyRelation.NoLazy);
            });

            ManyToOne(x => x.ParentProperty, m =>
            {
                m.Column("parentpropertyid");
                m.NotNullable(true);
                m.Cascade(Cascade.Persist);
                m.Fetch(FetchKind.Join);
                m.Lazy(LazyRelation.NoLazy);
            });

            ManyToOne(x => x.ChildProperty, m =>
            {
                m.Column("childpropertyid");
                m.NotNullable(true);
                m.Cascade(Cascade.Persist);
                m.Fetch(FetchKind.Join);
                m.Lazy(LazyRelation.NoLazy);
            });

            //Property(x => x.ReferenceId, m => m.NotNullable(true));
            //Property(x => x.ParentPropertyId, m => m.NotNullable(true));
            //Property(x => x.ChildPropertyId, m => m.NotNullable(true));
        }
    }
}
