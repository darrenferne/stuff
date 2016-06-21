using Brady.Trade.DataService.Abstract;
using Brady.Trade.Domain;

namespace Brady.Trade.DataService.Validators
{
    public class CommodityTradeDeleteValidator : CommodityTradeDeleteValidator<CommodityTrade>
    { }

    public class CommodityTradeDeleteValidator<T_trade> : TradeDeleteValidator<T_trade>
    { }
}