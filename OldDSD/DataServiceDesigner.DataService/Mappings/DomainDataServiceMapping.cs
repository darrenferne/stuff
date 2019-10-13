using DataServiceDesigner.Domain;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace DataServiceDesigner.DataService
{
    public class DomainDataServiceMap : ClassMapping<DomainDataService>
    {
        public DomainDataServiceMap()
        {
            Table("domaindataservice");

            Id(x => x.Id);

            Property(x => x.Name, m => m.NotNullable(true));
            
            ManyToOne(x => x.Connection, m =>
            {
                m.Column("connectionid");
                m.NotNullable(true);
                m.Cascade(Cascade.Persist);
                m.Fetch(FetchKind.Join);
                m.Lazy(LazyRelation.NoLazy);
            });

            Bag(x => x.Schemas,
                m =>
                {
                    m.Key(k => {
                        k.Column("dataserviceid");
                        k.ForeignKey("fk_domainschema1");
                        k.NotNullable(true); 
                    });
                    m.Cascade(Cascade.All);
                    m.Lazy(CollectionLazy.NoLazy);
                },
                r => r.OneToMany());
        }
    }
}
