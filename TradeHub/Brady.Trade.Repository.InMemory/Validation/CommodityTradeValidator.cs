using Brady.Trade.DataService.Core.Abstract;
using Brady.Trade.Domain;

namespace Brady.Trade.DataService.InMemory.Validation
{
    public class CommodityTradeValidator : CommodityTradeValidator<CommodityTrade>
    { }

    public class CommodityTradeValidator<T_Trade> : TradeDataServiceValidatorBase<T_Trade>
    { }
}