using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.DataServices.Notification
{
    public class DatabaseTypeElement : ConfigurationElement
    {
        private const string DATABASE_TYPE = "databaseType";
        private const string DATABASE_NAME = "databaseName";
        private const string CONNECTION_STRING = "connectionString";
        
        [ConfigurationProperty(DATABASE_TYPE, IsRequired = true)]
        public string DatabaseType
        {
            get { return (string)this[DATABASE_TYPE]; }
            set { this[DATABASE_TYPE] = value; }
        }

        [ConfigurationProperty(DATABASE_NAME, IsRequired = false)]
        public string DatabaseName
        {
            get { return (string)this[DATABASE_NAME]; }
            set { this[DATABASE_NAME] = value; }
        }

        [ConfigurationProperty(CONNECTION_STRING, IsRequired = false)]
        public string ConnectionString
        {
            get { return (string)this[CONNECTION_STRING]; }
            set { this[CONNECTION_STRING] = value; }
        }
    }
}
