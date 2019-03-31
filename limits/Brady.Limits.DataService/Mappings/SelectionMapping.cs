using NHibernate.Mapping.ByCode.Conformist;

namespace Brady.Limits.DataService.Mappings
{
    public class ProvisionalContractMapping : ClassMapping<Domain.Selection>
    {
        public ProvisionalContractMapping()
        {
            Id(p => p.Id);
            Property(p => p.Name);
            Property(p => p.Filter);
        }       
    }
}
