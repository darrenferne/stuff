using Brady.Trade.DataService.Core.Interfaces;
using BWF.DataServices.Support.NHibernate.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Trade.Repository
{
    public interface ICruddingTradeDataServiceRepository : ITradeDataServiceRepository, ICrudingDataServiceRepository
    {
    }
}
