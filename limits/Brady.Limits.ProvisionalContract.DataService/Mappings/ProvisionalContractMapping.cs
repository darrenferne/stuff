using Brady.Limits.ProvisionalContract.Domain;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Type;
using System;
using System.Collections.Generic;
using System.Text;

namespace Brady.Limits.ProvisionalContract.DataService.DataAccess.Mappings
{
    public class ProvisionalContractMapping : ClassMapping<Domain.ProvisionalContract>
    {
        public ProvisionalContractMapping()
        {
            Id(p => p.Id);
            Property(p => p.ContractId);
            Property(p => p.ClientNumber);
            Property(p => p.ClientName);
            Property(p => p.Product);
            Property(p => p.Quantity);
            Property(p => p.QuantityUnit);
            Property(p => p.Premium);
            Property(p => p.Status);
        }       
    }
}
