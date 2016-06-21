namespace Brady.Trade.Domain
{
    public interface IOptionTrade
    {
        IOptionDetails OptionDetails { get; set; }
    }
}