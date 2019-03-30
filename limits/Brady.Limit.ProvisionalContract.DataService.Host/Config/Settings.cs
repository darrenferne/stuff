using System.Configuration;

namespace Brady.Limits.ProvisionalContract.DataService.Host.Config
{
    public class Settings
    {
        public Settings()
        {
            ServiceName = "Brady.Limits.ProvisionalContract.DataService.host";
            ServiceDisplayName = "Brady Provisional Contract Data Service";
            ServiceDescription = "The host for the Brady Provisional Contact Data Service";

            HostUrl = ConfigurationManager.AppSettings["HostUrl"];
            ExplorerHostUrl = ConfigurationManager.AppSettings["ExplorerHostUrl"];
        }

        public string ServiceName { get; private set; }
        public string ServiceDisplayName { get; private set; }
        public string ServiceDescription { get; private set; }
        public string HostUrl { get; set; }
        public string ExplorerHostUrl { get; set; }
    }
}
