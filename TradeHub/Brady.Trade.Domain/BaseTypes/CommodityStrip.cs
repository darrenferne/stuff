using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Trade.Domain
{
    public class CommodityStrip<T_trade> : CommodityTrade where T_trade : CommodityTrade, new()
    {
        private List<T_trade> _legs;

        public CommodityStrip()
            : base("StripOf" + (new T_trade()).TradeType)
        {
            _legs = new List<T_trade>();
        }

        public IEnumerable<T_trade> Legs { get { return _legs; } }
    }
}
