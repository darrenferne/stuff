using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Trade.Domain
{
    public class Strip<T_trade> : Trade where T_trade : Trade, new()
    {
        private List<T_trade> _legs;

        public Strip()
            : base("StripOf" + (new T_trade()).TradeType)
        {
            _legs = new List<T_trade>();
        }

        public virtual IEnumerable<T_trade> Legs { get { return _legs; } }
    }
}
