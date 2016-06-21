using BWF.DataServices.Support.NHibernate.Abstract;
using BWF.DataServices.Support.NHibernate.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Trade.DataService.Abstract
{
    public abstract class TradeValidatorBase<T> : Validator<T>, IRequireCrudingDataServiceRepository
    {
        protected ITradeDataServiceRepository _repository;

        public TradeValidatorBase()
        { }

        public virtual void SetRepository(ICrudingDataServiceRepository repository)
        {
            _repository = repository as ITradeDataServiceRepository;
        }
    }
}
