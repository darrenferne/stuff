using BWF.DataServices.Core.Abstract;
using BWF.DataServices.Core.Interfaces;
using BWF.DataServices.Support.NHibernate.Abstract;
using BWF.Globalisation.Interfaces;
using BWF.Hosting.Infrastructure.Interfaces;
using FluentValidation;
using System.Collections.Generic;
using BWF.DataServices.Core.Menu;
using DataServiceDesigner.Domain;

namespace DataServiceDesigner.DataService
{
    public class DataServiceDesignerDataService : ConventionalDatabaseDataService<DataServiceDesignerDataService>, IDataServiceDesignerDataService
    {
        public DataServiceDesignerDataService(
            IHostConfiguration hostConfiguration,
            IEnumerable<IRecordType> recordTypes,
            IGlobalisationProvider globalisationProvider,
            IAuthorisation authorisation,
            IMetadataProvider metadataProvider,
            IDataServiceDesignerRepository repository,
            IDatabaseStreamingQueryExecutor databaseStreamingQueryExecutor)
        : base(
            Constants.DataServiceName,
            globalisationProvider,
            repository as DatabaseDataServiceRepository,
            recordTypes,
            metadataProvider,
            databaseStreamingQueryExecutor)
        {
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;
        }

        private MenuItem CreateMenuForModel(string model, string displayName, int position = 0)
        {
            return new MenuItem
            {
                Id = $"{Constants.DataServiceName}-{model}",
                Text = displayName,
                Position = position,
                Link = $"{{appSetting-ExplorerHostUrl}}/view/#default/{Constants.DataServiceName}/{model}"
            };
        }
        public override IEnumerable<MenuItem> GetCustomMenus()
        {
            var menu = new List<MenuItem>()
            {
                new MenuItem() {
                    Id = Constants.DataServiceName,
                    Text = Constants.DataServiceDisplayName,
                    Position = -1,
                    Items = new List<MenuItem>() {
                        CreateMenuForModel(nameof(DataServiceSolution), "Data Service Solution", 2),
                        CreateMenuForModel(nameof(DataServiceConnection), "Data Service Connections", 2),
                        CreateMenuForModel(nameof(DomainDataService), "Domain Data Services", 3),
                        CreateMenuForModel(nameof(DomainSchema), "Domain Schemas", 4),
                        CreateMenuForModel(nameof(DomainObject), "Domain Objects", 5),
                        CreateMenuForModel(nameof(DomainObjectProperty), "Domain Object Properties", 6),
                        CreateMenuForModel(nameof(DomainObjectReference), "Domain Object References", 7),
                        CreateMenuForModel(nameof(DomainObjectReferenceProperty), "Domain Object Reference Properties", 8)
                    }
                }
            };
            return menu;
        }
    }
}
