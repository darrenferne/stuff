using Brady.Trade.DataService.Core.Abstract;
using Brady.Trade.Domain.BaseTypes;

namespace Brady.Trade.DataService.Core.Validators
{
    public class AverageDetailsValidator : AverageDetailsValidator<AverageDetails>
    { }

    public class AverageDetailsValidator<T_average> : TradeDataServiceValidatorBase<T_average>
    { }
}