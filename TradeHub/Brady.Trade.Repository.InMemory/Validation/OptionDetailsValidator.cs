using Brady.Trade.DataService.Core.Abstract;
using Brady.Trade.Domain.BaseTypes;

namespace Brady.Trade.DataService.InMemory.Validation
{
    public class OptionDetailsValidator : AverageDetailsValidator<OptionDetails>
    { }

    public class OptionDetailsValidator<T_option> : TradeDataServiceValidatorBase<T_option>
    { }
}