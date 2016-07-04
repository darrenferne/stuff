using Brady.Trade.Domain.BaseTypes;

namespace Brady.Trade.DataService.InMemory.Validation
{
    public class AverageDetailsDeleteValidator : AverageDetailsDeleteValidator<AverageDetails>
    { }

    public class AverageDetailsDeleteValidator<T_average> : TradeDataServiceValidatorBase<T_average>
    { }
}