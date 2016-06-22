using AutoMapper;
using BWF.DataServices.Core.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Trade.DataService.Core.Abstract
{
    public class ReadOnlyTradeRecordType<T_trade> : RecordType<T_trade>
        where T_trade : Domain.Trade
    {
        public ReadOnlyTradeRecordType()
        { }

        public override void ConfigureMapper()
        {
            Mapper.CreateMap<T_trade, T_trade>();
        }
    }
}
