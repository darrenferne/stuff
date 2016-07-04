using Brady.Trade.DataService.Core.Abstract;
using Brady.Trade.Domain.BaseTypes;

namespace Brady.Trade.DataService.InMemory.Validation
{
    public class OptionDetailsDeleteValidator : OptionDetailsDeleteValidator<OptionDetails>
    { }

    public class OptionDetailsDeleteValidator<T_option> : TradeDataServiceValidatorBase<T_option>
    { }
}