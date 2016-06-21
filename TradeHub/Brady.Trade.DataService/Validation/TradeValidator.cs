using Brady.Trade.DataService.Abstract;

namespace Brady.Trade.DataService.Validators
{
    public class TradeValidator : TradeValidator<Domain.Trade>
    { }

    public class TradeValidator<T_trade> : TradeValidatorBase<T_trade>
    { }
}