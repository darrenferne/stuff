using BWF.DataServices.Metadata.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Trade.Domain
{
    public class Trade : IHaveCompositeId
    {
        public Trade()
        { }

        internal Trade(string tradeType)
        {
            TradeType = tradeType;
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

        string IHaveId<string>.Id
        {
            get { return ExternalId; }
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Trade);
        }

        public virtual bool Equals(Trade trade)
        {
            if (ReferenceEquals(trade, null))
                return false;

            if (ReferenceEquals(this, trade))
                return true;

            return string.Compare(this.SystemCode, trade.SystemCode) == 0 &&
                    string.Compare(this.SystemId, trade.SystemId) == 0;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = 17;
                result = result * 23 + ((this.SystemCode != null) ? this.SystemCode.GetHashCode() : 0);
                result = result * 23 + ((this.SystemId != null) ? this.SystemId.GetHashCode() : 0);
                return result;
            }
        }
    }
}
