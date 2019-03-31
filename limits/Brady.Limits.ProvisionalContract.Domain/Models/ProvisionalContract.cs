using BWF.DataServices.Metadata.Interfaces;
using System;

namespace Brady.Limits.ProvisionalContract.Domain
{
    public class ProvisionalContract : IHaveId<long>
    {
        public virtual long Id { get; set; }
        public virtual string ContractId { get; set; }
        public virtual string ClientNumber { get; set; }
        public virtual string ClientName { get; set; }
        public virtual string Product { get; set; }
        public virtual float Quantity { get; set; }
        public virtual string QuantityUnit { get; set; }
        public virtual float Premium { get; set; }
        public virtual int Status { get; set;}
    }
}
