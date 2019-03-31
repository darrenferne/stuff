﻿using DataServiceDesigner.Domain;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace DataServiceDesigner.DataService
{
    public class DomainDataServiceMap : ClassMapping<DomainDataService>
    {
        public DomainDataServiceMap()
        {
            Table("dataservice");

            Id(x => x.Id);

            Property(x => x.Name);
            Property(x => x.DefaultSchema);

            ManyToOne(x => x.Connection, m =>
            {
                m.Column("connectionid");
                m.Cascade(Cascade.Persist);
                m.Lazy(LazyRelation.NoLazy);
                m.Fetch(FetchKind.Join);
            });

            Bag(x => x.DomainObjects, 
                m =>
                {
                    m.Key(k => k.Column("dataserviceid"));
                    m.Cascade(Cascade.All.Include(Cascade.DeleteOrphans));
                    m.Lazy(CollectionLazy.NoLazy);
                }, 
                r => r.OneToMany());
        }
    }
}