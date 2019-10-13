using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.DataServices.Notification
{
    public class TableElement : ConfigurationElement
    {
        private const string DATABASE_TYPE = "databaseType";
        private const string DATABASE_NAME = "databaseName";
        private const string TABLE = "table";
        private const string KEY = "key";
        private const string POLL_INTERVAL = "pollInterval";
        private const string NOTIFICATION_MECHANISM = "notificationMechanism";
        private const string NOTIFICATION_LEVEL = "notificationLevel";
        private const string OVERRIDES = "Overrides";

        [ConfigurationProperty(DATABASE_TYPE, IsRequired = false)]
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

        [ConfigurationProperty(TABLE, IsRequired = true)]
        public string Table
        {
            get { return (string)this[TABLE]; }
            set { this[TABLE] = value; }
        }

        [ConfigurationProperty(KEY, IsRequired = false)]
        public string Key
        {
            get { return (string)this[KEY]; }
            set { this[KEY] = value; }
        }

        [ConfigurationProperty(POLL_INTERVAL, IsRequired = false, DefaultValue = 5000)]
        [IntegerValidator(MinValue = 0, MaxValue = 86400000)]
        public int PollInterval
        {
            get { return (int)this[POLL_INTERVAL]; }
            set { this[POLL_INTERVAL] = value; }
        }

        [ConfigurationProperty(NOTIFICATION_MECHANISM, IsRequired = false, DefaultValue = NotificationMechanism.None)]
        public NotificationMechanism NotificationMechanism
        {
            get { return (NotificationMechanism)this[NOTIFICATION_MECHANISM]; }
            set { this[NOTIFICATION_MECHANISM] = value; }
        }

        [ConfigurationProperty(NOTIFICATION_LEVEL, IsRequired = false, DefaultValue = NotificationLevel.None)]
        public NotificationLevel NotificationLevel
        {
            get { return (NotificationLevel)this[NOTIFICATION_LEVEL]; }
            set { this[NOTIFICATION_LEVEL] = value; }
        }

        [ConfigurationProperty(OVERRIDES, IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(TableElementCollection), AddItemName = "add")]
        public TableElementCollection Overrides
        {
            get
            {
                return (TableElementCollection)base[OVERRIDES];
            }
        }
    }
}
