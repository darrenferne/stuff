﻿using Brady.Trade.DataService.Core.Abstract;

namespace Brady.Trade.DataService.InMemory.Validation
{
    public class TradeValidator : TradeValidator<Domain.Trade>
    { }

    public class TradeValidator<T_trade> : TradeDataServiceValidatorBase<T_trade>
    { }
}