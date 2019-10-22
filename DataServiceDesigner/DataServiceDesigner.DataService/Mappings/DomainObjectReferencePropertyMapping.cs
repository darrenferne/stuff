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
                m.Fetch(FetchKind.Join);
                m.Lazy(LazyRelation.NoLazy);
                m.Insert(false);
                m.Update(false);
            });

            ManyToOne(x => x.ParentProperty, m =>
            {
                m.Column("parentpropertyid");
                m.Fetch(FetchKind.Join);
                m.Lazy(LazyRelation.NoLazy);
                m.Insert(false);
                m.Update(false);
            });

            ManyToOne(x => x.ChildProperty, m =>
            {
                m.Column("childpropertyid");
                m.Fetch(FetchKind.Join);
                m.Lazy(LazyRelation.NoLazy);
                m.Insert(false);
                m.Update(false);
            });

            Property(x => x.ReferenceId, m => m.NotNullable(true));
            Property(x => x.ParentPropertyId, m => m.NotNullable(true));
            Property(x => x.ChildPropertyId, m => m.NotNullable(true));
        }
    }
}
