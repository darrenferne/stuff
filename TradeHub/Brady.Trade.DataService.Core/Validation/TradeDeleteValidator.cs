using Brady.Trade.DataService.Core.Abstract;

namespace Brady.Trade.DataService.Core.Validators
{
    public class TradeDeleteValidator : TradeDeleteValidator<Domain.Trade>
    { }

    public class TradeDeleteValidator<T_trade> : TradeDataServiceValidatorBase<T_trade>
    { }
}