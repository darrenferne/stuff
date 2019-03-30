using Brady.Limits.ExplorerHost.Config;
using BWF.DataServices.StartUp.Concrete;
using BWF.Explorer.StartUp.Concrete;
using BWF.Explorer.StartUp.Interfaces;
using BWF.Globalisation.Concrete;
using BWF.Globalisation.Interfaces;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.ExplorerHost
{
    class Program
    {
        static void Main(string[] args)
        {
            var availableCultures = new Cultures();
            var settings = new Settings();

            XmlConfigurator.Configure();

            var hostUrl = settings.ExplorerHostUrl;
            var defaultLink = settings.DefaultLink;

            IResourceQuerier resourceQuerier = new ResourceQuerier(typeof(BWF.Globalisation.Resources).Assembly);
            var globalisationProvider = new GlobalisationProvider(resourceQuerier, availableCultures.LanguageCultures, availableCultures.FormattingCultures);

            IExplorerHostSettings explorerHostSettings = new ExplorerHostSettings(settings.ServiceName, settings.ServiceDisplayName, settings.ServiceDescription, hostUrl, defaultLink, globalisationProvider);
            var explorerHost = new BWF.Explorer.StartUp.Concrete.ExplorerHost(explorerHostSettings);

            explorerHost.Start();
        }
    }
}
