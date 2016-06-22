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
                trade.OptionDetails = new VanillaOptionDetails();

            return trade;
        }
        #endregion

        #region Commodity Average Extensions
        public static CommodityAverage AsVanillaAverage(this CommodityAverage trade)
        {
            if (trade.AverageDetails == null)
                trade.AverageDetails = new VanillaAverageDetails();

            return trade;
        }
        #endregion

        #region Commodity TAPO Extensions
        public static CommodityTAPO AsVanillaAverage(this CommodityTAPO trade)
        {
            if (trade.AverageDetails == null)
                trade.AverageDetails = new VanillaAverageDetails();

            return trade;
        }

        public static CommodityTAPO AsVanillaOption(this CommodityTAPO trade)
        {
            if (trade.OptionDetails == null)
                trade.OptionDetails = new VanillaOptionDetails();

            return trade;
        }
        #endregion

        #region Commodity Average Swap Extensions
        public static CommodityAverageSwap AsVanillaAverage(this CommodityAverageSwap trade)
        {
            if (trade.AverageDetails == null)
                trade.AverageDetails = new VanillaAverageDetails();

            return trade;
        }
        #endregion

        #region Vanilla Option Extensions
        public static VanillaOptionDetails CurrencyAmount(this VanillaOptionDetails option, decimal? currencyAmount)
        {
            option.CurrencyAmount = currencyAmount;
            return option;
        }

        public static IOptionTrade CurrencyAmount(this IOptionTrade option, decimal? currencyAmount)
        {
            if (option.OptionDetails is VanillaOptionDetails)
            {
                ((VanillaOptionDetails)option.OptionDetails).CurrencyAmount(currencyAmount);
            }

            return option;
        }

        public static VanillaOptionDetails CP(this VanillaOptionDetails option, string cp)
        {
            option.CP = cp;
            return option;
        }

        public static IOptionTrade CP(this IOptionTrade option, string cp)
        {

            if (option.OptionDetails is VanillaOptionDetails)
            {
                ((VanillaOptionDetails)option.OptionDetails).CP(cp);
            }
            return option;
        }

        public static VanillaOptionDetails StrikePrice(this VanillaOptionDetails option, decimal? strikePrice)
        {
            option.StrikePrice = strikePrice;
            return option;
        }

        public static IOptionTrade StrikePrice(this IOptionTrade option, decimal? strikePrice)
        {
            if (option.OptionDetails is VanillaOptionDetails)
            {
                ((VanillaOptionDetails)option.OptionDetails).StrikePrice(strikePrice);
            }
            return option;
        }

        public static VanillaOptionDetails Model(this VanillaOptionDetails option, string model)
        {
            option.Model = model;
            return option;
        }

        public static IOptionTrade Model(this IOptionTrade option, string model)
        {
            if (option.OptionDetails is VanillaOptionDetails)
            {
                ((VanillaOptionDetails)option.OptionDetails).Model(model);
            }
            return option;
        }

        public static VanillaOptionDetails ExpiryMonth(this VanillaOptionDetails option, DateTime expiryMonth)
        {
            option.ExpiryMonth = expiryMonth;
            return option;
        }

        public static IOptionTrade ExpiryMonth(this IOptionTrade option, DateTime expiryMonth)
        {
            if (option.OptionDetails is VanillaOptionDetails)
            {
                ((VanillaOptionDetails)option.OptionDetails).ExpiryMonth(expiryMonth);
            }
            return option;
        }

        public static VanillaOptionDetails ExpiryDate(this VanillaOptionDetails option, DateTime expiryDate)
        {
            option.ExpiryDate = expiryDate;
            return option;
        }

        public static IOptionTrade ExpiryDate(this IOptionTrade option, DateTime expiryDate)
        {
            if (option.OptionDetails is VanillaOptionDetails)
            {
                ((VanillaOptionDetails)option.OptionDetails).ExpiryDate(expiryDate);
            }
            return option;
        }

        public static VanillaOptionDetails PremiumDate(this VanillaOptionDetails option, DateTime premiumDate)
        {
            option.PremiumDate = premiumDate;
            return option;
        }

        public static IOptionTrade PremiumDate(this IOptionTrade option, DateTime premiumDate)
        {
            if (option.OptionDetails is VanillaOptionDetails)
            {
                ((VanillaOptionDetails)option.OptionDetails).PremiumDate(premiumDate);
            }
            return option;
        }

        public static VanillaOptionDetails PremiumCurrency(this VanillaOptionDetails option, string premiumCurrency)
        {
            option.PremiumCurrency = premiumCurrency;
            return option;
        }

        public static IOptionTrade PremiumCurrency(this IOptionTrade option, string premiumCurrency)
        {
            if (option.OptionDetails is VanillaOptionDetails)
            {
                ((VanillaOptionDetails)option.OptionDetails).PremiumCurrency(premiumCurrency);
            }
            return option;
        }

        public static VanillaOptionDetails PremiumRate(this VanillaOptionDetails option, decimal? premiumRate)
        {
            option.PremiumRate = premiumRate;
            return option;
        }

        public static IOptionTrade PremiumRate(this IOptionTrade option, decimal? premiumRate)
        {
            if (option.OptionDetails is VanillaOptionDetails)
            {
                ((VanillaOptionDetails)option.OptionDetails).PremiumRate(premiumRate);
            }
            return option;
        }

        public static VanillaOptionDetails PremiumAmount(this VanillaOptionDetails option, decimal? premiumAmount)
        {
            option.PremiumAmount = premiumAmount;
            return option;
        }

        public static IOptionTrade PremiumAmount(this IOptionTrade option, decimal? premiumAmount)
        {
            if (option.OptionDetails is VanillaOptionDetails)
            {
                ((VanillaOptionDetails)option.OptionDetails).PremiumAmount(premiumAmount);
            }
            return option;
        }
        #endregion

        #region Vanilla Average Details
        public static VanillaAverageDetails StartDate(this VanillaAverageDetails average, DateTime startDate)
        {
            average.StartDate = startDate;
            return average;
        }

        public static IAverageTrade StartDate(this IAverageTrade average, DateTime startDate)
        {
            if (average.AverageDetails is VanillaAverageDetails)
            {
                ((VanillaAverageDetails)average.AverageDetails).StartDate(startDate);
            }
            return average;
        }

        public static VanillaAverageDetails EndDate(this VanillaAverageDetails average, DateTime endDate)
        {
            average.EndDate = endDate;
            return average;
        }

        public static IAverageTrade EndDate(this IAverageTrade average, DateTime endDate)
        {
            if (average.AverageDetails is VanillaAverageDetails)
            {
                ((VanillaAverageDetails)average.AverageDetails).EndDate(endDate);
            }
            return average;
        }

        public static VanillaAverageDetails FixingIndex(this VanillaAverageDetails average, string fixingIndex)
        {
            average.FixingIndex = fixingIndex;
            return average;
        }

        public static IAverageTrade FixingIndex(this IAverageTrade average, string fixingIndex)
        {
            if (average.AverageDetails is VanillaAverageDetails)
            {
                ((VanillaAverageDetails)average.AverageDetails).FixingIndex(fixingIndex);
            }
            return average;
        }

        public static VanillaAverageDetails AdditivePremium(this VanillaAverageDetails average, decimal additivePremium)
        {
            average.AdditivePremium = additivePremium;
            return average;
        }

        public static IAverageTrade AdditivePremium(this IAverageTrade average, decimal additivePremium)
        {
            if (average.AverageDetails is VanillaAverageDetails)
            {
                ((VanillaAverageDetails)average.AverageDetails).AdditivePremium(additivePremium);
            }
            return average;
        }

        public static VanillaAverageDetails AdditivePremiumUnits(this VanillaAverageDetails average, string additivePremiumUnits)
        {
            average.AdditivePremiumUnits = additivePremiumUnits;
            return average;
        }

        public static IAverageTrade AdditivePremiumUnits(this IAverageTrade average, string additivePremiumUnits)
        {
            if (average.AverageDetails is VanillaAverageDetails)
            {
                ((VanillaAverageDetails)average.AverageDetails).AdditivePremiumUnits(additivePremiumUnits);
            }
            return average;
        }

        public static VanillaAverageDetails PercentagePremium(this VanillaAverageDetails average, decimal percentagePremium)
        {
            average.PercentagePremium = percentagePremium;
            return average;
        }

        public static IAverageTrade PercentagePremium(this IAverageTrade average, decimal percentagePremium)
        {
            if (average.AverageDetails is VanillaAverageDetails)
            {
                ((VanillaAverageDetails)average.AverageDetails).PercentagePremium(percentagePremium);
            }
            return average;
        }

        public static VanillaAverageDetails IsFixedPrice(this VanillaAverageDetails average, bool isFixedPrice)
        {
            average.IsFixedPrice = isFixedPrice;
            return average;
        }

        public static IAverageTrade IsFixedPrice(this IAverageTrade average, bool isFixedPrice)
        {
            if (average.AverageDetails is VanillaAverageDetails)
            {
                ((VanillaAverageDetails)average.AverageDetails).IsFixedPrice(isFixedPrice);
            }
            return average;
        }

        public static VanillaAverageDetails FixedPrice(this VanillaAverageDetails average, decimal fixedPrice)
        {
            average.FixedPrice = fixedPrice;
            return average;
        }

        public static IAverageTrade FixedPrice(this IAverageTrade average, decimal fixedPrice)
        {
            if (average.AverageDetails is VanillaAverageDetails)
            {
                ((VanillaAverageDetails)average.AverageDetails).FixedPrice(fixedPrice);
            }
            return average;
        }
        #endregion

    }
}
