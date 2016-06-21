using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Trade.Domain
{
    public static class CommodityTradeExtensions
    {
        #region Commodity Trade Extensions
        public static Trade BS(this CommodityTrade trade, string bs)
        {
            trade.BS = bs;
            return trade;
        }

        public static Trade Term(this CommodityTrade trade, string term)
        {
            trade.Term = term;
            return trade;
        }

        public static Trade DeliveryMonth(this CommodityTrade trade, DateTime deliveryMonth)
        {
            trade.DeliveryMonth = deliveryMonth;
            return trade;
        }

        public static Trade DeliveryDate(this CommodityTrade trade, DateTime deliveryDate)
        {
            trade.DeliveryDate = deliveryDate;
            return trade;
        }

        public static Trade Lots(this CommodityTrade trade, decimal lots)
        {
            trade.Lots = lots;
            return trade;
        }

        public static Trade CommodityAmount(this CommodityTrade trade, decimal commodityAmount)
        {
            trade.CommodityAmount = commodityAmount;
            return trade;
        }

        public static Trade CommodityUnits(this CommodityTrade trade, string commodityUnits)
        {
            trade.CommodityUnits = commodityUnits;
            return trade;
        }
        #endregion

        #region Commodity Future Extensions
        public static Trade CurrencyAmount(this CommodityFuture trade, decimal? currencyAmount)
        {
            trade.CurrencyAmount = currencyAmount;
            return trade;
        }

        public static Trade BasePrice(this CommodityFuture trade, decimal? basePrice)
        {
            trade.BasePrice = basePrice;
            return trade;
        }

        public static Trade Spread(this CommodityFuture trade, decimal? spread)
        {
            trade.Spread = spread;
            return trade;
        }

        public static Trade Price(this CommodityFuture trade, decimal? price)
        {
            trade.Price = price;
            return trade;
        }
        #endregion

        #region Commodity Forward Extensions
        public static Trade CurrencyAmount(this CommodityForward trade, decimal? currencyAmount)
        {
            trade.CurrencyAmount = currencyAmount;
            return trade;
        }

        public static Trade BasePrice(this CommodityForward trade, decimal? basePrice)
        {
            trade.BasePrice = basePrice;
            return trade;
        }

        public static Trade Spread(this CommodityForward trade, decimal? spread)
        {
            trade.Spread = spread;
            return trade;
        }

        public static Trade Price(this CommodityForward trade, decimal? price)
        {
            trade.Price = price;
            return trade;
        }
        #endregion

        #region Commodity Option Extensions
        public static CommodityOption AsVanillaOption(this CommodityOption trade)
        {
            if (trade.OptionDetails == null)
                trade.OptionDetails = new VanillaOption();

            return trade;
        }
        #endregion

        #region Commodity Average Extensions
        public static CommodityAverage AsVanillaAverage(this CommodityAverage trade)
        {
            if (trade.AverageDetails == null)
                trade.AverageDetails = new VanillaAverage();

            return trade;
        }
        #endregion

        #region Commodity TAPO Extensions
        public static CommodityTAPO AsVanillaAverage(this CommodityTAPO trade)
        {
            if (trade.AverageDetails == null)
                trade.AverageDetails = new VanillaAverage();

            return trade;
        }

        public static CommodityTAPO AsVanillaOption(this CommodityTAPO trade)
        {
            if (trade.OptionDetails == null)
                trade.OptionDetails = new VanillaOption();

            return trade;
        }
        #endregion

        #region Commodity Average Swap Extensions
        public static CommodityAverageSwap AsVanillaAverage(this CommodityAverageSwap trade)
        {
            if (trade.AverageDetails == null)
                trade.AverageDetails = new VanillaAverage();

            return trade;
        }
        #endregion

        #region Vanilla Option Extensions
        public static VanillaOption CurrencyAmount(this VanillaOption option, decimal? currencyAmount)
        {
            option.CurrencyAmount = currencyAmount;
            return option;
        }

        public static IOptionTrade CurrencyAmount(this IOptionTrade option, decimal? currencyAmount)
        {
            if (option.OptionDetails is VanillaOption)
            {
                ((VanillaOption)option.OptionDetails).CurrencyAmount(currencyAmount);
            }

            return option;
        }

        public static VanillaOption CP(this VanillaOption option, string cp)
        {
            option.CP = cp;
            return option;
        }

        public static IOptionTrade CP(this IOptionTrade option, string cp)
        {

            if (option.OptionDetails is VanillaOption)
            {
                ((VanillaOption)option.OptionDetails).CP(cp);
            }
            return option;
        }

        public static VanillaOption StrikePrice(this VanillaOption option, decimal? strikePrice)
        {
            option.StrikePrice = strikePrice;
            return option;
        }

        public static IOptionTrade StrikePrice(this IOptionTrade option, decimal? strikePrice)
        {
            if (option.OptionDetails is VanillaOption)
            {
                ((VanillaOption)option.OptionDetails).StrikePrice(strikePrice);
            }
            return option;
        }

        public static VanillaOption Model(this VanillaOption option, string model)
        {
            option.Model = model;
            return option;
        }

        public static IOptionTrade Model(this IOptionTrade option, string model)
        {
            if (option.OptionDetails is VanillaOption)
            {
                ((VanillaOption)option.OptionDetails).Model(model);
            }
            return option;
        }

        public static VanillaOption ExpiryMonth(this VanillaOption option, DateTime expiryMonth)
        {
            option.ExpiryMonth = expiryMonth;
            return option;
        }

        public static IOptionTrade ExpiryMonth(this IOptionTrade option, DateTime expiryMonth)
        {
            if (option.OptionDetails is VanillaOption)
            {
                ((VanillaOption)option.OptionDetails).ExpiryMonth(expiryMonth);
            }
            return option;
        }

        public static VanillaOption ExpiryDate(this VanillaOption option, DateTime expiryDate)
        {
            option.ExpiryDate = expiryDate;
            return option;
        }

        public static IOptionTrade ExpiryDate(this IOptionTrade option, DateTime expiryDate)
        {
            if (option.OptionDetails is VanillaOption)
            {
                ((VanillaOption)option.OptionDetails).ExpiryDate(expiryDate);
            }
            return option;
        }

        public static VanillaOption PremiumDate(this VanillaOption option, DateTime premiumDate)
        {
            option.PremiumDate = premiumDate;
            return option;
        }

        public static IOptionTrade PremiumDate(this IOptionTrade option, DateTime premiumDate)
        {
            if (option.OptionDetails is VanillaOption)
            {
                ((VanillaOption)option.OptionDetails).PremiumDate(premiumDate);
            }
            return option;
        }

        public static VanillaOption PremiumCurrency(this VanillaOption option, string premiumCurrency)
        {
            option.PremiumCurrency = premiumCurrency;
            return option;
        }

        public static IOptionTrade PremiumCurrency(this IOptionTrade option, string premiumCurrency)
        {
            if (option.OptionDetails is VanillaOption)
            {
                ((VanillaOption)option.OptionDetails).PremiumCurrency(premiumCurrency);
            }
            return option;
        }

        public static VanillaOption PremiumRate(this VanillaOption option, decimal? premiumRate)
        {
            option.PremiumRate = premiumRate;
            return option;
        }

        public static IOptionTrade PremiumRate(this IOptionTrade option, decimal? premiumRate)
        {
            if (option.OptionDetails is VanillaOption)
            {
                ((VanillaOption)option.OptionDetails).PremiumRate(premiumRate);
            }
            return option;
        }

        public static VanillaOption PremiumAmount(this VanillaOption option, decimal? premiumAmount)
        {
            option.PremiumAmount = premiumAmount;
            return option;
        }

        public static IOptionTrade PremiumAmount(this IOptionTrade option, decimal? premiumAmount)
        {
            if (option.OptionDetails is VanillaOption)
            {
                ((VanillaOption)option.OptionDetails).PremiumAmount(premiumAmount);
            }
            return option;
        }
        #endregion

        #region Vanilla Average Details
        public static VanillaAverage StartDate(this VanillaAverage average, DateTime startDate)
        {
            average.StartDate = startDate;
            return average;
        }

        public static IAverageTrade StartDate(this IAverageTrade average, DateTime startDate)
        {
            if (average.AverageDetails is VanillaAverage)
            {
                ((VanillaAverage)average.AverageDetails).StartDate(startDate);
            }
            return average;
        }

        public static VanillaAverage EndDate(this VanillaAverage average, DateTime endDate)
        {
            average.EndDate = endDate;
            return average;
        }

        public static IAverageTrade EndDate(this IAverageTrade average, DateTime endDate)
        {
            if (average.AverageDetails is VanillaAverage)
            {
                ((VanillaAverage)average.AverageDetails).EndDate(endDate);
            }
            return average;
        }

        public static VanillaAverage FixingIndex(this VanillaAverage average, string fixingIndex)
        {
            average.FixingIndex = fixingIndex;
            return average;
        }

        public static IAverageTrade FixingIndex(this IAverageTrade average, string fixingIndex)
        {
            if (average.AverageDetails is VanillaAverage)
            {
                ((VanillaAverage)average.AverageDetails).FixingIndex(fixingIndex);
            }
            return average;
        }

        public static VanillaAverage AdditivePremium(this VanillaAverage average, decimal additivePremium)
        {
            average.AdditivePremium = additivePremium;
            return average;
        }

        public static IAverageTrade AdditivePremium(this IAverageTrade average, decimal additivePremium)
        {
            if (average.AverageDetails is VanillaAverage)
            {
                ((VanillaAverage)average.AverageDetails).AdditivePremium(additivePremium);
            }
            return average;
        }

        public static VanillaAverage AdditivePremiumUnits(this VanillaAverage average, string additivePremiumUnits)
        {
            average.AdditivePremiumUnits = additivePremiumUnits;
            return average;
        }

        public static IAverageTrade AdditivePremiumUnits(this IAverageTrade average, string additivePremiumUnits)
        {
            if (average.AverageDetails is VanillaAverage)
            {
                ((VanillaAverage)average.AverageDetails).AdditivePremiumUnits(additivePremiumUnits);
            }
            return average;
        }

        public static VanillaAverage PercentagePremium(this VanillaAverage average, decimal percentagePremium)
        {
            average.PercentagePremium = percentagePremium;
            return average;
        }

        public static IAverageTrade PercentagePremium(this IAverageTrade average, decimal percentagePremium)
        {
            if (average.AverageDetails is VanillaAverage)
            {
                ((VanillaAverage)average.AverageDetails).PercentagePremium(percentagePremium);
            }
            return average;
        }

        public static VanillaAverage IsFixedPrice(this VanillaAverage average, bool isFixedPrice)
        {
            average.IsFixedPrice = isFixedPrice;
            return average;
        }

        public static IAverageTrade IsFixedPrice(this IAverageTrade average, bool isFixedPrice)
        {
            if (average.AverageDetails is VanillaAverage)
            {
                ((VanillaAverage)average.AverageDetails).IsFixedPrice(isFixedPrice);
            }
            return average;
        }

        public static VanillaAverage FixedPrice(this VanillaAverage average, decimal fixedPrice)
        {
            average.FixedPrice = fixedPrice;
            return average;
        }

        public static IAverageTrade FixedPrice(this IAverageTrade average, decimal fixedPrice)
        {
            if (average.AverageDetails is VanillaAverage)
            {
                ((VanillaAverage)average.AverageDetails).FixedPrice(fixedPrice);
            }
            return average;
        }
        #endregion

    }
}
