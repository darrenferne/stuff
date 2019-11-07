using DataServiceDesigner.Domain;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace DataServiceDesigner.DataService
{
    public class DataServiceSolutionMap : ClassMapping<DataServiceSolution>
    {
        public DataServiceSolutionMap()
        {
            Table("solution");

            Id(x => x.Id);

            Property(x => x.Name);
            Property(x => x.NamespacePrefix);
            Property(x => x.ServiceName);
            Property(x => x.ServiceDisplayName);
            Property(x => x.ServiceDescription);
            Property(x => x.CompanyName);

            Bag(x => x.DataServices,
                m =>
                {
                    m.Key(k => {
                        k.Column("solutionid");
                        k.NotNullable(true);
                    });
                    m.Cascade(Cascade.All.Include(Cascade.DeleteOrphans));
                    m.Lazy(CollectionLazy.NoLazy);
                    m.Inverse(true);
                },
                r => r.OneToMany());
        }
    }
}
