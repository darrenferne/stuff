using BWF.DataServices.Core.Interfaces;
using BWF.DataServices.Support.NHibernate.Interfaces;
using DataServiceDesigner.Domain;

namespace DataServiceDesigner.DataService
{
    public interface IDataServiceDesignerRepository : ICrudingDataServiceRepository
    {
        //DesignerDataService GetDataService(long id);
        //DesignerConnection GetConnection(long id);
    }
}