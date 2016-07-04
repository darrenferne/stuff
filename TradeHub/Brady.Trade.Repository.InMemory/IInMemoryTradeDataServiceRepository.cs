using Brady.Trade.DataService.Core.Interfaces;
using Brady.Trade.Domain.Interfaces;
using BWF.DataServices.Core.Interfaces;
using BWF.DataServices.Support.NHibernate.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Trade.Repository
{
    public interface IInMemoryTradeDataServiceRepository : ITradeDataServiceRepository
    {
        T_item Get<T_item>(long Id) where T_item : class, IHaveAssignableId<long>;
        IEnumerable<T_item> GetWhere<T_item>(Predicate<T_item> filter) where T_item : class, IHaveAssignableId<long>;
        T_item Create<T_item>(T_item item) where T_item : class, IHaveAssignableId<long>;
        void Update<T_item>(T_item item) where T_item : class, IHaveAssignableId<long>;
        void Delete<T_item>(long id) where T_item : class, IHaveAssignableId<long>;
    }
}
