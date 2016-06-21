using Brady.Trade.DataService.Abstract;
using Brady.Trade.Domain;

namespace Brady.Trade.DataService.Validators
{
    public class CommodityTradeValidator : CommodityTradeValidator<CommodityTrade>
    { }

    public class CommodityTradeValidator<T_Trade> : TradeValidatorBase<T_Trade>
    { }
}