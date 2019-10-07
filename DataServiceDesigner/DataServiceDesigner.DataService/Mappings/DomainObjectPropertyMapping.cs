using DataServiceDesigner.Domain;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace DataServiceDesigner.DataService
{
    public class DomainObjectPropertyMap : ClassMapping<DomainObjectProperty>
    {
        public DomainObjectPropertyMap()
        {
            Table("domainobjectproperty");

            Id(x => x.Id);

            Property(x => x.DbName);
            Property(x => x.Name);
            Property(x => x.DisplayName);
            Property(x => x.IsPartOfKey);
            Property(x => x.IncludeInDefaultView);

            ManyToOne(x => x.DataService, m =>
            {
                m.Column("dataserviceid");
                m.NotNullable(true);
                m.Cascade(Cascade.Persist);
                m.Lazy(LazyRelation.NoLazy);
                m.Fetch(FetchKind.Join);
            });

            ManyToOne(x => x.Object, m =>
            {
                m.Column("objectid");
                m.NotNullable(true);
                m.Cascade(Cascade.Persist);
                m.Lazy(LazyRelation.NoLazy);
                m.Fetch(FetchKind.Join);
            });
        }
    }
}
