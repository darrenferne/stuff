using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace Brady.DataServices.Notification
{
    public class NotificationConfiguration : ConfigurationSection
    {
        private const string SECTION_NAME = "Brady.DataServices.Notification";
        private const string POLL_INTERVAL = "defaultPollInterval";
        private const string NOTIFICATION_MECHANISM = "defaultNotificationMechanism";
        private const string NOTIFICATION_LEVEL = "defaultNotificationLevel";
        private const string DATABASES = "Databases";
        private const string TABLE_OVERRIDES = "TableOverrides";
        private const string PRE_LOAD_TABLES = "PreLoadTables";

        private static NotificationConfiguration _currentConfiguration;

        public static NotificationConfiguration Open()
        {
            NotificationConfiguration section = null;

            section = (NotificationConfiguration)ConfigurationManager.GetSection(SECTION_NAME);
            if (section == null)
            {
                //open the config file
                System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                if (config != null)
                {
                    //open the publisher section of the config
                    section = (NotificationConfiguration)config.GetSection(SECTION_NAME);
                }
            }
      
            if (section == null)
                section = new NotificationConfiguration();

            _currentConfiguration = section;

            return section;
        }

        public static NotificationConfiguration Current
        {
            get 
            {
                if (_currentConfiguration == null)
                    _currentConfiguration = NotificationConfiguration.Open();
                return _currentConfiguration; 
            }
        }

        [ConfigurationProperty(POLL_INTERVAL, IsRequired = false, DefaultValue = 0)]
        [IntegerValidator(MinValue = 0, MaxValue = 86400000)]
        public int DefaultPollInterval
        {
            get { return (int)this[POLL_INTERVAL]; }
            set { this[POLL_INTERVAL] = value; }
        }

        [ConfigurationProperty(NOTIFICATION_MECHANISM, IsRequired = false)]
        public NotificationMechanism DefaultNotificationMechanism
        {
            get { return (NotificationMechanism)this[NOTIFICATION_MECHANISM]; }
            set { this[NOTIFICATION_MECHANISM] = value; }
        }

        [ConfigurationProperty(NOTIFICATION_LEVEL, IsRequired = false)]
        public NotificationLevel DefaultNotificationLevel
        {
            get { return (NotificationLevel)this[NOTIFICATION_LEVEL]; }
            set { this[NOTIFICATION_LEVEL] = value; }
        }

        [ConfigurationProperty(DATABASES, IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(DatabaseTypeElementCollection), AddItemName = "add")]
        public DatabaseTypeElementCollection Databases
        {
            get
            {
                return (DatabaseTypeElementCollection)base[DATABASES];
            }
        }

        [ConfigurationProperty(TABLE_OVERRIDES, IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(TableElementCollection), AddItemName = "add")]
        public TableElementCollection TableOverrides
        {
            get
            {
                return (TableElementCollection)base[TABLE_OVERRIDES];
            }
        }

        [ConfigurationProperty(PRE_LOAD_TABLES, IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(TableElementCollection), AddItemName = "add")]
        public TableElementCollection PreLoadedTables
        {
            get
            {
                return (TableElementCollection)base[PRE_LOAD_TABLES];
            }
        }
    }
}
