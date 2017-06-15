using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.DataServices.Notification
{
    public class DatabaseTypeElementCollection : ConfigurationElementCollection
    {
        private object GetKey(string databaseType, string databaseName)
        {
            return string.Format("{0}:{1}", databaseType, databaseName);
        }

        public string GetConnectionString(string databaseType, string databaseName)
        {
            var element = base.BaseGet(GetKey(databaseType, databaseName)) as DatabaseTypeElement;
            if (element == null)
                return string.Empty;
            else
                return element.ConnectionString;
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new DatabaseTypeElement();
        }

        public DatabaseTypeElement this[int index]
        {
            get
            {
                return (DatabaseTypeElement)BaseGet(index);
            }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        public int IndexOf(DatabaseTypeElement type)
        {
            return BaseIndexOf(type);
        }

        public void Add(DatabaseTypeElement type)
        {
            BaseAdd(type);
        }

        public void Remove(DatabaseTypeElement type)
        {
            if (BaseIndexOf(type) >= 0)
                BaseRemove((ConfigurationElement)type);
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        public void Remove(string name)
        {
            BaseRemove(name);
        }

        public void Clear()
        {
            BaseClear();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            DatabaseTypeElement dt = element as DatabaseTypeElement;
            return GetKey(dt.DatabaseType, dt.DatabaseName);
        }
    }
}
