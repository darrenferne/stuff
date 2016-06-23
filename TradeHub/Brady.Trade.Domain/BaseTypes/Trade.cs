using BWF.DataServices.Metadata.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Trade.Domain
{
    public class Trade : IHaveId<long>
    {
        public Trade()
        { }

        internal Trade(string tradeType)
        {
            TradeType = tradeType;
        }

        public virtual long Id
        {
            get { return RepositoryId; }
            set { RepositoryId = value; }
        }
        public virtual long RepositoryId { get; set; }
        public virtual string ExternalId { get; protected internal set; }
        public virtual string SystemCode { get; set; }
        public virtual string SystemId { get; set; }
        public virtual string TradeType { get; protected internal set; }
        public virtual string ContractCode { get; set; }
        public virtual string MarketCode { get; set; }
        public virtual bool? IsLive { get; set; }
        public virtual string Entity { get; set; }
        public virtual string Counterparty { get; set; }
        public virtual string Portfolio { get; set; }
        public virtual string TradedBy { get; set; }
        public virtual DateTime? TradedOn { get; set; }
        public virtual string EnteredBy { get; set; }
        public virtual DateTime? EnteredOn { get; set; }
        public virtual string UpdatedBy { get; set; }
        public virtual DateTime? UpdateOn { get; set; }
        public virtual DateTime? EffectiveDate { get; set; }

        public virtual Dictionary<string, object> ExtendedProperties { get; set; }
    }
}
