using Brady.Trade.DataService.Abstract;

namespace Brady.Trade.DataService.Validators
{
    public class TradeDeleteValidator : TradeDeleteValidator<Domain.Trade>
    { }

    public class TradeDeleteValidator<T_trade> : TradeValidatorBase<T_trade>
    { }
}