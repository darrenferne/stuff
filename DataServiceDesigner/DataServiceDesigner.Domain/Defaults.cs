using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServiceDesigner.Domain
{
    public static class Defaults
    {
        private static string _namespacePrefix;

        private static string _companyName;
        public static string NamespacePrefix 
        {
            get 
            {
                if (string.IsNullOrEmpty(_namespacePrefix))
                    _namespacePrefix = ConfigurationManager.AppSettings.Get(nameof(NamespacePrefix));
                if (string.IsNullOrEmpty(_namespacePrefix))
                    _namespacePrefix = "Brady";

                return _namespacePrefix;
            }
        }

        public static string CompanyName
        {
            get 
            {
                if (string.IsNullOrEmpty(_companyName))
                    _companyName = ConfigurationManager.AppSettings.Get(nameof(CompanyName));
                if (string.IsNullOrEmpty(_namespacePrefix))
                    _companyName = "Brady PLC";

                return _companyName;
            }
        }
    }
}
