using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.DataServices.Notification
{
    public class TableElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new TableElement();
        }

        public TableElement GetTable(string name, string databaseType, string databaseName)
        {
            foreach(TableElement table in this)
            {
                if (string.Compare(table.Table, name, true) == 0)
                {
                    if (string.IsNullOrEmpty(table.DatabaseType) || string.Compare(table.DatabaseType, databaseType, true) == 0)
                    {
                        if (string.IsNullOrEmpty(table.DatabaseName) || string.Compare(table.DatabaseName, databaseName, true) == 0)
                            return (TableElement)BaseGet((object)name);
                    }
                }
            }
            return null;
        }

        public TableElement this[int index]
        {
            get
            {
                return (TableElement)BaseGet(index);
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

        public int IndexOf(TableElement type)
        {
            return BaseIndexOf(type);
        }

        public void Add(TableElement type)
        {
            BaseAdd(type);
        }

        public void Remove(TableElement type)
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
            TableElement lt = element as TableElement;
            return lt.Table;
        }
    }
}
