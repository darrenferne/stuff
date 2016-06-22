using Brady.Trade.DataService.Core.Abstract;
using Brady.Trade.Domain.BaseTypes;

namespace Brady.Trade.DataService.Core.Validators
{
    public class AverageDetailsDeleteValidator : AverageDetailsDeleteValidator<AverageDetails>
    { }

    public class AverageDetailsDeleteValidator<T_average> : TradeDataServiceValidatorBase<T_average>
    { }
}