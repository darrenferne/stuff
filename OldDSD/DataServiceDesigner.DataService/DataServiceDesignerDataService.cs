using BWF.DataServices.Core.Abstract;
using BWF.DataServices.Core.Interfaces;
using BWF.DataServices.Support.NHibernate.Abstract;
using BWF.Globalisation.Interfaces;
using BWF.Hosting.Infrastructure.Interfaces;
using FluentValidation;
using System.Collections.Generic;
using BWF.DataServices.Core.Menu;

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

        public override IEnumerable<MenuItem> GetCustomMenus()
        {
            var menu = new List<MenuItem>()
            {
                new MenuItem() {
                    Id = "DataServiceDesigner",
                    Text = "Data Service Designer",
                    Position = -1,
                    Items = new List<MenuItem>() {
                        new MenuItem()
                        {
                            Id = "DomainDataService",
                            Text = "Domain Data Services",
                            Link = "{{appSetting-ExplorerHostUrl}}/view/#default/dataservicedesigner/DomainDataService",
                            Position = 1
                        },
                        new MenuItem()
                        {
                            Id = "DomainConnection",
                            Text = "Domain Connections",
                            Link = "{{appSetting-ExplorerHostUrl}}/view/#default/dataservicedesigner/DataServiceConnection",
                            Position = 2
                        },
                        new MenuItem()
                        {
                            Id = "DomainSchema",
                            Text = "Domain Schemas",
                            Link = "{{appSetting-ExplorerHostUrl}}/view/#default/dataservicedesigner/DomainSchema",
                            Position = 3
                        },
                        new MenuItem()
                        {
                            Id = "DomainObject",
                            Text = "Domain Objects",
                            Link = "{{appSetting-ExplorerHostUrl}}/view/#default/dataservicedesigner/DomainObject",
                            Position = 4
                        },
                        new MenuItem()
                        {
                            Id = "DomainObjectProperty",
                            Text = "Domain Object Properties",
                            Link = "{{appSetting-ExplorerHostUrl}}/view/#default/dataservicedesigner/DomainObjectProperty",
                            Position = 5
                        }
                    }
                }
            };
            return menu;
        }
    }
}
