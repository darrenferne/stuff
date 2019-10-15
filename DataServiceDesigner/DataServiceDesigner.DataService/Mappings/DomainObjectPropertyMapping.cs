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

            ManyToOne(x => x.Object, m =>
            {
                m.Column("objectid");
                m.NotNullable(true);
                m.Cascade(Cascade.Persist);
                m.Fetch(FetchKind.Join);
                m.Lazy(LazyRelation.NoLazy);
            });

            Property(x => x.ColumnName);
            Property(x => x.PropertyName);
            Property(x => x.DisplayName);
            Property(x => x.PropertyType);
            Property(x => x.Length);
            Property(x => x.IsNullable);
            Property(x => x.IsPartOfKey);
            Property(x => x.IncludeInDefaultView);
        }
    }
}
