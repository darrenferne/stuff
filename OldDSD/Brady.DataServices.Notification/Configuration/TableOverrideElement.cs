using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.DataServices.Notification
{
    public class TableOverrideElement : ConfigurationElement
    {
        private const string TABLE = "Table";
        private const string OVERRIDES = "Overrides";
        private const string POLL_INTERVAL = "PollInterval";
        private const string POLL_MECHANISM = "PollMechanism";

        [ConfigurationProperty(TABLE, IsRequired = true)]
        public string Table
        {
            get { return (string)this[TABLE]; }
            set { this[TABLE] = value; }
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

        [ConfigurationProperty(POLL_INTERVAL, IsRequired = false, DefaultValue = 5000)]
        [IntegerValidator(MinValue = 0, MaxValue = 86400000)]
        public int PollInterval
        {
            get { return (int)this[POLL_INTERVAL]; }
            set { this[POLL_INTERVAL] = value; }
        }

        [ConfigurationProperty(POLL_MECHANISM, IsRequired = false)]
        public NotificationMechanism PollMechanism
        {
            get { return (NotificationMechanism)this[POLL_MECHANISM]; }
            set { this[POLL_MECHANISM] = value; }
        }
    }
}
