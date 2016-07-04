using Brady.Trade.DataService.Core.Abstract;
using Brady.Trade.Domain.BaseTypes;

namespace Brady.Trade.DataService.Database.Validation
{
    public class OptionDetailsValidator : OptionDetailsValidator<OptionDetails>
    { }

    public class OptionDetailsValidator<T_option> : TradeDataServiceValidatorBase<T_option>
    { }
}