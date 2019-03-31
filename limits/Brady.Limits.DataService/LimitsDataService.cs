using Brady.Limits.Domain;
using BWF.DataServices.Core.Abstract;
using BWF.DataServices.Core.Interfaces;
using BWF.DataServices.Core.Menu;
using BWF.DataServices.Metadata.Attributes.Actions;
using BWF.DataServices.Nancy.Interfaces;
using BWF.DataServices.Support.NHibernate.Abstract;
using BWF.Globalisation.Interfaces;
using FluentValidation;
using System.Collections.Generic;

namespace Brady.Limits.DataService
{
    [ImportAction()]
    [ExportAction()]
    public class LimitsDataService : ConventionalDatabaseDataService<LimitsDataService>, ILimitsDataService
    {
        IDataServiceHostSettings _hostSettings;
        string _hostUrl;

        public LimitsDataService(ILimitsDataServiceRepository repository, IDataServiceHostSettings hostSettings, IEnumerable<IRecordType> recordTypes, IAuthorisation authorisation, IGlobalisationProvider globalisationProvider, IMetadataProvider metadataProvider, IDatabaseStreamingQueryExecutor streamingQueryExecutor)
            : base(Constants.DataServiceName, globalisationProvider, repository as DatabaseDataServiceRepository, recordTypes, metadataProvider, streamingQueryExecutor)
        {
            _hostSettings = hostSettings;
            _hostUrl = hostSettings.HostUrl;

            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;
        }

        public override IEnumerable<MenuItem> GetCustomMenus()
        {
            var menuList = new List<MenuItem>();
            var limitsMenu = new MenuItem
            {
                Id = "limitsprototype",
                Text = "Limits",
                Items = new List<MenuItem>
                {
                    new MenuItem
                    {
                        Id = "selections",
                        Text = "Selections",
                        Link = $"{_hostUrl}/view/#default/{Constants.DataServiceName}/Selection",
                        Position = 1
                    },
                    new MenuItem
                    {
                        Id = "workflows",
                        Text = "Workflows",
                        Link = $"{_hostUrl}/view/#default/{Constants.DataServiceName}/Workflow",
                        Position = 1
                    }
                }
            };
            menuList.Add(limitsMenu);
            return menuList;
        }
    }
}
