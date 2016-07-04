using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Trade.DataService.InMemory.Validation
{
    public abstract class TradeDataServiceValidatorBase<T> : AbstractValidator<T>
    {
        public TradeDataServiceValidatorBase()
        { }
    }
}
