using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Trade.DataService.Core.Interfaces
{
    public interface ITradeDataServiceSettings
    {
        string DataServiceName { get; set; }
        string DefaultSchema { get; set; }
    }
}
