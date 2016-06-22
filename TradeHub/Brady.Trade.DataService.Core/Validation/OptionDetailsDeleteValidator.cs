using Brady.Trade.DataService.Core.Abstract;
using Brady.Trade.Domain.BaseTypes;

namespace Brady.Trade.DataService.Core.Validators
{
    public class OptionDetailsDeleteValidator : OptionDetailsDeleteValidator<OptionDetails>
    { }

    public class OptionDetailsDeleteValidator<T_option> : TradeDataServiceValidatorBase<T_option>
    { }
}