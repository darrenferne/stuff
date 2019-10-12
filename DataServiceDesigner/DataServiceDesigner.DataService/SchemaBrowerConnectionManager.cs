using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BWF.DataServices.Core.Concrete.ChangeSets;
using BWF.DataServices.Core.Interfaces.ChangeSets;
using BWF.DataServices.Domain.Interfaces;
using BWF.DataServices.Domain.Models;
using DataServiceDesigner.Domain;
using Ninject;
using SchemaBrowser.DataService;
using SchemaBrowser.Domain;

namespace DataServiceDesigner.DataService
{
    public class SchemaBrowerConnectionManager : ISchemaBrowserConnectionManager
    {
        IKernel _kernel;
        ISchemaBrowserDataService _sbDataService;
        IChangeableRecordType<long, DbConnection> _dbConnectionRecordType;

        public SchemaBrowerConnectionManager(IKernel kernel)
        {
            _kernel = kernel;
        }

        bool Initialise()
        {
            if (_sbDataService is null)
            {
                try
                {
                    _sbDataService = _kernel.Get<ISchemaBrowserDataService>();
                    _dbConnectionRecordType = _sbDataService.GetRecordType(nameof(DbConnection)) as IChangeableRecordType<long, DbConnection>;
                }
                catch
                {
                    _sbDataService = null;
                    _dbConnectionRecordType = null;
                }
            }
            return !(_sbDataService is null);
        }

        public void SynchConnections(IChangeSet changeSet, string token)
        {
            if (Initialise())
            {
                var dsConnectionChangeSet = changeSet as ChangeSet<long, DataServiceConnection>;
                var sbConnectionChangeSet = _dbConnectionRecordType.GetNewChangeSet() as ChangeSet<long, DbConnection>;

                var settings = new ProcessChangeSetSettings(token);
                var itemRef = 1L;

                foreach (DataServiceConnection dsConnection in dsConnectionChangeSet.Create.Values)
                {
                    var sbConnection = new DbConnection
                    {
                        ExternalId = dsConnection.Id,
                        Name = dsConnection.Name,
                        DatabaseType = dsConnection.DatabaseType,
                        DataSource = dsConnection.DataSource,
                        InitialCatalog = dsConnection.InitialCatalog,
                        Username = dsConnection.Username,
                        Password = dsConnection.Password,
                        UseIntegratedSecurity = dsConnection.UseIntegratedSecurity,
                        ConnectionString = dsConnection.ConnectionString
                    };
                    sbConnectionChangeSet.AddCreate(itemRef, sbConnection, Enumerable.Empty<long>(), Enumerable.Empty<long>());
                    itemRef++;
                }

                _dbConnectionRecordType.ProcessChangeSet(_sbDataService, sbConnectionChangeSet, settings);
            }
        }
    }
}
