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
                            Id = "Connection",
                            Text = "Connections",
                            Link = "{{appSetting-ExplorerHostUrl}}/view/#default/dataservicedesigner/DesignerConnection",
                            Position = 1
                        },
                        new MenuItem()
                        {
                            Id = "DataService",
                            Text = "Data Services",
                            Link = "{{appSetting-ExplorerHostUrl}}/view/#default/dataservicedesigner/DesignerDataService",
                            Position = 2
                        },
                        new MenuItem()
                        {
                            Id = "DomainObject",
                            Text = "Domain Objects",
                            Link = "{{appSetting-ExplorerHostUrl}}/view/#default/dataservicedesigner/DesignerDomainObject",
                            Position = 3
                        },
                        new MenuItem()
                        {
                            Id = "DomainObjectProperty",
                            Text = "Domain Object Properties",
                            Link = "{{appSetting-ExplorerHostUrl}}/view/#default/dataservicedesigner/DesignerDomainObjectProperty",
                            Position = 4
                        }
                    }
                }
            };
            return menu;
        }
    }
}
