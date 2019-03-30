using Brady.Limits.ProvisionalContract.Domain;
using BWF.DataServices.Core.Abstract;
using BWF.DataServices.Core.Interfaces;
using BWF.DataServices.Core.Menu;
using BWF.DataServices.Metadata.Attributes.Actions;
using BWF.DataServices.Nancy.Interfaces;
using BWF.DataServices.Support.NHibernate.Abstract;
using BWF.Globalisation.Interfaces;
using BWF.Hosting.Infrastructure.Interfaces;
using FluentValidation;
using System;
using System.Collections.Generic;

namespace Brady.Limits.ProvisionalContract.DataService
{
    [ImportAction()]
    [ExportAction()]
    public class ProvisionalContractDataService : ConventionalDatabaseDataService<ProvisionalContractDataService>, IProvisionalContractDataService
    {
        IDataServiceHostSettings _hostSettings;
        string _hostUrl;

        public ProvisionalContractDataService(IProvisionalContractDataServiceRepository repository, IDataServiceHostSettings hostSettings, IEnumerable<IRecordType> recordTypes, IAuthorisation authorisation, IGlobalisationProvider globalisationProvider, IMetadataProvider metadataProvider, IDatabaseStreamingQueryExecutor streamingQueryExecutor)
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
                Id = "limits",
                Text = "Limits",
                Items = new List<MenuItem>
                {
                    new MenuItem
                    {
                        Id = "provisional-contracts",
                        Text = "Provisional Contracts",
                        Link = $"{_hostUrl}/view/#default/{Constants.DataServiceName}/ProvisionalContract",
                        Position = 1
                    }
                }
            };
            menuList.Add(limitsMenu);
            return menuList;
        }
    }
}