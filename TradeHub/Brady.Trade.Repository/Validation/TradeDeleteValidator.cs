using Brady.Trade.DataService.Core.Abstract;

namespace Brady.Trade.DataService.Database.Validation
{
    public class TradeDeleteValidator : TradeDeleteValidator<Domain.Trade>
    { }

    public class TradeDeleteValidator<T_trade> : TradeDataServiceValidatorBase<T_trade>
    { }
}