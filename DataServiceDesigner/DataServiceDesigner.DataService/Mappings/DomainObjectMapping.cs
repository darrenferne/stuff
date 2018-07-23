using DataServiceDesigner.Domain;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace DataServiceDesigner.DataService
{
    public class DomainObjectMap : ClassMapping<DomainObject>
    {
        public DomainObjectMap()
        {
            Table("domainobject");

            Id(x => x.Id);

            Property(x => x.DbSchemaName, m => m.Column("DbSchema"));
            Property(x => x.DbObjectName, m => m.Column("DbName"));
            Property(x => x.Name);
            Property(x => x.DisplayName);
            Property(x => x.PluralisedDisplayName);

            ManyToOne(x => x.DataService, m =>
            {
                m.Column("dataserviceid");
                m.Cascade(Cascade.Persist);
                m.Lazy(LazyRelation.NoLazy);
                m.Fetch(FetchKind.Join);
            });

            Bag(x => x.ObjectProperties,
                m =>
                {
                    m.Key(k => k.Column("objectid"));
                    m.Cascade(Cascade.All.Include(Cascade.DeleteOrphans));
                    m.Lazy(CollectionLazy.NoLazy);
                },
                r => r.OneToMany());
        }
    }
}
