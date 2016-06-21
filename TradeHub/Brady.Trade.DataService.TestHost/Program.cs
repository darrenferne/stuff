using Brady.Trade.DataService.TestHost.Config;
using BWF.DataServices.StartUp.Concrete;
using BWF.Explorer.StartUp.Concrete;
using BWF.Explorer.StartUp.Interfaces;
using BWF.Globalisation.Concrete;
using BWF.Globalisation.Interfaces;
using log4net.Config;
using System;
using System.Reflection;

namespace Brady.Trade.DataService.TestHost
{
    class Program
    {
        static void Main(string[] args)
        {
            var availableCultures = new Cultures();
            var settings = new Settings();

            XmlConfigurator.Configure();

            var hostUrl = settings.ExplorerHostUrl;

            IResourceQuerier resourceQuerier = new ResourceQuerier(typeof(BWF.Globalisation.Resources).Assembly);
            var globalisationProvider = new GlobalisationProvider(resourceQuerier, availableCultures.LanguageCultures, availableCultures.FormattingCultures);
            var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var menuFile = "Brady.Trade.DataService.TestHost.Menu.Menu.json";
            var menuProvider = new JsonMenuProvider(menuFile, Assembly.GetExecutingAssembly());

            IExplorerHostSettings explorerHostSettings = new ExplorerHostSettings("Brady.Trade.DataService.TestHost", "Brady Trade Data Service Host", "Host for the Brady Trade Data Service", hostUrl, menuProvider, globalisationProvider);
            var explorerHost = new BWF.Explorer.StartUp.Concrete.ExplorerHost(explorerHostSettings);

            explorerHost.Start();
        }
    }
}
