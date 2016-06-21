using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Trade.Domain
{
    public static class TradeExtensions
    {
        public static Trade TradeType(this Trade trade, string tradeType)
        {
            trade.TradeType = tradeType;
            return trade;
        }

        public static Trade ContractCode(this Trade trade, string contractCode)
        {
            trade.ContractCode = contractCode;
            return trade;
        }

        public static Trade MarketCode(this Trade trade, string marketCode)
        {
            trade.MarketCode =marketCode;
            return trade;
        }

        public static Trade SystemCode(this Trade trade, string systemCode)
        {
            trade.SystemCode = systemCode;
            return trade;
        }

        public static Trade SystemId(this Trade trade, string systemId)
        {
            trade.SystemId = systemId;
            return trade;
        }

        public static Trade IsLive(this Trade trade, bool isLive)
        {
            trade.IsLive = isLive;
            return trade;
        }

        public static Trade Entity(this Trade trade, string entity)
        {
            trade.Entity = entity;
            return trade;
        }

        public static Trade Counterparty(this Trade trade, string counterparty)
        {
            trade.Counterparty = counterparty;
            return trade;
        }

        public static Trade Portfolio(this Trade trade, string portfolio)
        {
            trade.Portfolio = portfolio;
            return trade;
        }

        public static Trade TradedBy(this Trade trade, string tradedBy)
        {
            trade.TradedBy = tradedBy;
            return trade;
        }

        public static Trade TradedOn(this Trade trade, DateTime tradedOn) 
        {
            trade.TradedOn = tradedOn;
            return trade;
        }

        public static Trade EnteredOn(this Trade trade, DateTime enteredOn)
        {
            trade.EnteredOn = enteredOn;
            return trade;
        }

        public static Trade UpdateOn(this Trade trade, DateTime enteredOn)
        {
            trade.EnteredOn = enteredOn;
            return trade;
        }

        public static Trade Extend(this Trade trade, string key, object value)
        {
            if (trade.ExtendedProperties == null)
                trade.ExtendedProperties = new Dictionary<string, object>();

            trade.ExtendedProperties.Add(key, value);

            return trade;
        }
    }
}
