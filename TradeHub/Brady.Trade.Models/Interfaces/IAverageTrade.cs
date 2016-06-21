using Brady.Trade.Domain.BaseTypes;

namespace Brady.Trade.Domain
{
    public interface IAverageTrade
    {
        AverageDetails AverageDetails { get; set; }
    }
}