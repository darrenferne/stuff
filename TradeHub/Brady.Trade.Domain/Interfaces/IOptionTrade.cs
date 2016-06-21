using Brady.Trade.Domain.BaseTypes;

namespace Brady.Trade.Domain
{
    public interface IOptionTrade
    {
        OptionDetails OptionDetails { get; set; }
    }
}