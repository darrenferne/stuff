using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Trade.Domain
{
    public class TradeIdentifier
    {
        public string SystemCode { get; set; }
        public string SystemId { get; set; }

        public override string ToString()
        {
            return $"{this.SystemCode};{this.SystemId}";
        }

        public virtual bool Equals(TradeIdentifier id)
        {
            if (ReferenceEquals(id, null))
                return false;

            if (ReferenceEquals(this, id))
                return true;

            return string.Compare(this.SystemCode, id.SystemCode) == 0 &&
                    string.Compare(this.SystemId, id.SystemId) == 0;
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
