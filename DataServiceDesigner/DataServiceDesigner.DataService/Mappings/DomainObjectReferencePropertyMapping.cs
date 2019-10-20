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

            ManyToOne(x => x.Parent, m =>
            {
                m.Column("parentpropertyid");
                m.NotNullable(true);
                m.Fetch(FetchKind.Join);
                m.Lazy(LazyRelation.NoLazy);
            });

            ManyToOne(x => x.Child, m =>
            {
                m.Column("childpropertyid");
                m.NotNullable(true);
                m.Fetch(FetchKind.Join);
                m.Lazy(LazyRelation.NoLazy);
            });
        }
    }
}
