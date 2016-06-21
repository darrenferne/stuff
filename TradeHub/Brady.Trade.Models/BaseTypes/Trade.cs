using BWF.DataServices.Metadata.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Trade.Domain
{
    public abstract class Trade : IHaveCompositeId
    {
        public Trade(string tradeType)
        {
            TradeType = tradeType;
        }

        public string Id { get; set; }
        public string SystemCode { get; set; }
        public string SystemId { get; set; }
        public string TradeType { get; internal set; }
        public string ContractCode { get; set; }
        public string MarketCode { get; set; }
        public bool? IsLive { get; set; }
        public string Entity { get; set; }
        public string Counterparty { get; set; }
        public string Portfolio { get; set; }
        public string TradedBy { get; set; }
        public DateTime? TradedOn { get; set; }
        public string EnteredBy { get; set; }
        public DateTime? EnteredOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdateOn { get; set; }
        public DateTime? EffectiveDate { get; set; }

        public Dictionary<string, object> ExtendedProperties { get; set; }

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
