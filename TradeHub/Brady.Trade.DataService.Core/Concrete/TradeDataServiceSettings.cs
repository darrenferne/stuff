using Brady.Trade.DataService.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Trade.DataService.Core.Concrete
{
    public class TradeDataServiceSettings : ITradeDataServiceSettings
    {
        public TradeDataServiceSettings()
        {
            DataServiceName = "trading";
            DefaultSchema = "brady_trading";
        }

        public string DataServiceName { get; set; }
        public string DefaultSchema { get; set; }
    }
}
