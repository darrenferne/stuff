using System.Configuration;

namespace Brady.Trade.DataService.TestHost.Config
{
    public class Settings
    {
        public readonly string ExplorerHostUrl;
        public readonly string RootUrl;

        public Settings()
        {
            var appSettingsReader = new AppSettingsReader();
            ExplorerHostUrl = appSettingsReader.GetValue("ExplorerHostUrl", typeof(string)).ToString();
            RootUrl = appSettingsReader.GetValue("RootUrl", typeof(string)).ToString();
        }
    }
}
