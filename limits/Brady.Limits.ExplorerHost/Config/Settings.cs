using System.Configuration;

namespace Brady.Limits.ExplorerHost.Config
{
    public class Settings
    {
        public readonly string ExplorerHostUrl;
        public readonly string DefaultLink;

        public readonly string ServiceName;
        public readonly string ServiceDisplayName;
        public readonly string ServiceDescription;

        public Settings()
        {

            ServiceName = "Brady.Limits.ExplorerHost";
            ServiceDisplayName = "Brady Limits Explorer Host";
            ServiceDescription = "Host for the Brady Limits Service";

            var appSettingsReader = new AppSettingsReader();

            ExplorerHostUrl = appSettingsReader.GetValue("ExplorerHostUrl", typeof(string)).ToString();
            DefaultLink = appSettingsReader.GetValue("DefaultLink", typeof(string)).ToString();
        }
    }
}
