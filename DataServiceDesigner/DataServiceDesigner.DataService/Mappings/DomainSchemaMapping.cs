using DataServiceDesigner.Domain;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace DataServiceDesigner.DataService
{
    public class DomainSchemaMap : ClassMapping<DomainSchema>
    {
        public DomainSchemaMap()
        {
            Table("domainschema");

            Id(x => x.Id);

            ManyToOne(x => x.DataService, m =>
            {
                m.Column("dataserviceid");
                m.NotNullable(true);
                m.Cascade(Cascade.Persist);
                m.Fetch(FetchKind.Join);
                m.Lazy(LazyRelation.NoLazy);
            });

            Property(x => x.SchemaName, m => m.NotNullable(true));
            Property(x => x.IsDefault);

            Bag(x => x.Objects,
                m =>
                {
                    m.Key(k => k.Column("schemaid"));
                    m.Cascade(Cascade.All);
                    m.Lazy(CollectionLazy.NoLazy);
                    m.Inverse(true);
                },
                r => r.OneToMany());

            Bag(x => x.References,
                m =>
                {
                    m.Key(k => k.Column("schemaid"));
                    m.Cascade(Cascade.All);
                    m.Lazy(CollectionLazy.NoLazy);
                    m.Inverse(true);
                },
                r => r.OneToMany());
        }
    }
}
