using Brady.Limits.ProvisionalContract.DataService.Host.Config;
using BWF.DataServices.StartUp.Concrete;
using BWF.Globalisation.Concrete;
using BWF.Globalisation.Interfaces;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf.Runtime;

namespace Brady.Limit.ProvisionalContract.DataService.Host
{
    class Program
    {
        static void Main(string[] args)
        {
            XmlConfigurator.Configure();

            var cultures = new Cultures();
            IResourceQuerier resourceQuerier =
                new ResourceQuerier(
                    typeof(BWF.Globalisation.Resources).Assembly);

            var globalisationProvider = new GlobalisationProvider(resourceQuerier,
                                                                cultures.LanguageCultures,
                                                                cultures.FormattingCultures);

            var hostSettings = new Settings();
            var dataserviceHostSettings =
                new DataServiceHostSettings(
                    hostSettings.ServiceName,
                    hostSettings.ServiceDisplayName,
                    hostSettings.ServiceDescription,
                    hostSettings.HostUrl,
                    null, //let discovery handle this
                    globalisationProvider);

            var host = new DataServiceHost(dataserviceHostSettings);

            host.Start();
        }
    }
}
