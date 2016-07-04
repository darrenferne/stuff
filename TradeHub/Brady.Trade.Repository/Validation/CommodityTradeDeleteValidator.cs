using Brady.Trade.DataService.Core.Abstract;
using Brady.Trade.Domain;

namespace Brady.Trade.DataService.Database.Validation
{
    public class CommodityTradeDeleteValidator : CommodityTradeDeleteValidator<CommodityTrade>
    { }

    public class CommodityTradeDeleteValidator<T_trade> : TradeDeleteValidator<T_trade>
    { }
}