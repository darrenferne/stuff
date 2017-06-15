using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.DataServices.Notification
{
    public class TableOverrideElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new TableOverrideElement();
        }

        public TableOverrideElement GetTable(string table)
        {
            return (TableOverrideElement)BaseGet((object)table);
        }

        public TableOverrideElement this[int index]
        {
            get
            {
                return (TableOverrideElement)BaseGet(index);
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

        public int IndexOf(TableOverrideElement type)
        {
            return BaseIndexOf(type);
        }

        public void Add(TableOverrideElement type)
        {
            BaseAdd(type);
        }

        public void Remove(TableOverrideElement type)
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
            TableOverrideElement to = element as TableOverrideElement;
            return to.Table;
        }
    }
}
