using BWF.DataServices.Core.Abstract;
using BWF.DataServices.Core.Interfaces;
using BWF.Globalisation.Interfaces;
using FluentValidation;
using System.Collections.Generic;
using BWF.DataServices.Core.Menu;

namespace SchemaBrowser.DataService
{
    public class SchemaBrowserDataService : InMemoryDataService<SchemaBrowserDataService>, ISchemaBrowserDataService
    {
        public SchemaBrowserDataService(
            IEnumerable<IRecordType> recordTypes,
            IGlobalisationProvider globalisationProvider,
            IAuthorisation authorisation,
            IMetadataProvider metadataProvider,
            ISchemaBrowserRepository repository)
        : base(
            Constants.DataServiceName,
            globalisationProvider,
            repository as InMemoryDataServiceRepository,
            recordTypes,
            authorisation,
            metadataProvider)
        {
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;
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
                        new MenuItem()
                        {
                            Id = "DbConnections",
                            Text = "Db Connections",
                            Link = "{{appSetting-ExplorerHostUrl}}/view/#default/" + Constants.DataServiceName + "/DbConnection",
                            Position = 1
                        },
                        new MenuItem()
                        {
                            Id = "DbSchemas",
                            Text = "Db Schemas",
                            Link = "{{appSetting-ExplorerHostUrl}}/view/#default/" + Constants.DataServiceName + "/DbSchema",
                            Position = 2
                        },
                        new MenuItem()
                        {
                            Id = "DbObjects",
                            Text = "Db Objects",
                            Link = "{{appSetting-ExplorerHostUrl}}/view/#default/" + Constants.DataServiceName + "/DbObject",
                            Position = 3
                        },
                        new MenuItem()
                        {
                            Id = "DbObjectProperties",
                            Text = "Db Object Properties",
                            Link = "{{appSetting-ExplorerHostUrl}}/view/#default/" + Constants.DataServiceName + "/DbObjectProperty",
                            Position = 4
                        },
                        new MenuItem()
                        {
                            Id = "DbObjectPrimaryKeys",
                            Text = "Db Object Primary Keys",
                            Link = "{{appSetting-ExplorerHostUrl}}/view/#default/" + Constants.DataServiceName + "/DbObjectPrimaryKey",
                            Position = 4
                        },
                        new MenuItem()
                        {
                            Id = "DbObjectForeignKeys",
                            Text = "Db Object Foreign Keys",
                            Link = "{{appSetting-ExplorerHostUrl}}/view/#default/" + Constants.DataServiceName + "/DbObjectForeignKey",
                            Position = 4
                        }
                    }
                }
            };
            return menu;
        }
    }
}
