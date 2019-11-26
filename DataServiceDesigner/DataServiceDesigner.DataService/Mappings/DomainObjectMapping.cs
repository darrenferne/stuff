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

            ManyToOne(x => x.Schema, m =>
            {
                m.Column("schemaid");
                m.NotNullable(true);
                m.Cascade(Cascade.Persist);
                m.Fetch(FetchKind.Join);
                m.Lazy(LazyRelation.NoLazy);
            });

            Property(x => x.TableName, m => m.NotNullable(true));
            Property(x => x.ObjectName, m => m.NotNullable(true));
            Property(x => x.DisplayName);
            Property(x => x.PluralisedDisplayName);

            Bag(x => x.Properties,
                m =>
                {
                    m.Key(k => k.Column("objectid"));
                    m.Cascade(Cascade.All.Include(Cascade.DeleteOrphans));
                    m.Lazy(CollectionLazy.NoLazy);
                    m.Inverse(true);
                },
                r => r.OneToMany());
        }
    }
}
