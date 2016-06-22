using Brady.Trade.DataService.Core.Interfaces;
using BWF.DataServices.Support.NHibernate.Abstract;
using BWF.DataServices.Support.NHibernate.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Trade.DataService.Core.Abstract
{
    public abstract class TradeDataServiceValidatorBase<T> : Validator<T>, IRequireCrudingDataServiceRepository
    {
        protected ITradeDataServiceRepository _repository;

        public TradeDataServiceValidatorBase()
        { }

        public virtual void SetRepository(ICrudingDataServiceRepository repository)
        {
            _repository = repository as ITradeDataServiceRepository;
        }
    }
}
