using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.DataServices.Notification
{
    public class NotificationTable : INotificationQuery
    {
        public const string DefaultTableAlias = "t";

        string _name;
        string _sql;

        public NotificationTable()
        { }

        public NotificationTable(string name, NotificationMechanism notificationMechanism, NotificationLevel notificationLevel, int pollInterval, string tableName, string keyFieldName, string[] columnNames = null)
            : this(name, notificationMechanism, notificationLevel, pollInterval, tableName, new string[] { keyFieldName }, columnNames)
        { }

        public NotificationTable(string name, NotificationMechanism notificationMechanism, NotificationLevel notificationLevel, int pollInterval, string tableName, string[] keyFieldNames, string[] columnNames = null)
        {
            Name = name;
            NotificationMechanism = notificationMechanism;
            NotificationLevel = notificationLevel;
            PollInterval = pollInterval;
            TableName = tableName;
            KeyFieldNames = keyFieldNames;
            ColumnNames = columnNames;
        }

        public NotificationMechanism NotificationMechanism { get; set; }
        public NotificationLevel NotificationLevel { get; set; }
        public int PollInterval { get; set; }
        public string TableName { get; set; }
        public string[] KeyFieldNames { get; set; }
        public string[] ColumnNames { get; set; }

        public string Name
        {
            get 
            { 
                if (string.IsNullOrEmpty(_name))
                    _name = TableName;
                return _name; 
            }
            set { _name = value; }
        }

        public string Sql
        {
            get 
            {
                if (string.IsNullOrEmpty(_sql))
                    _sql = GetTableSql(DefaultTableAlias);
                return _sql;
            }
            set { _sql = value; }
        }

        public string GetTableSql(string tableAlias, params string[] injectColumns)
        {
            string sql = "SELECT ";
            int columnCount = 0;

            if (string.IsNullOrEmpty(tableAlias))
                tableAlias = DefaultTableAlias;

            if (ColumnNames == null) 
            {
                sql += tableAlias + ".*";
                columnCount++;
            }
            else if (ColumnNames.Length != 0)
            {
                //build the column list
                sql += tableAlias + "." + ColumnNames[0];
                for (int c = 1; c < ColumnNames.Length; c++)
                    sql += ", " + tableAlias + "." + ColumnNames[c];
                columnCount += ColumnNames.Length;
            }
            
            //inject the key fields if not already present
            if (KeyFieldNames != null)
            {
                for (int k = 0; k < this.KeyFieldNames.Length; k++)
                {
                    if (ColumnNames == null || !ColumnNames.Contains(KeyFieldNames[k]))
                    {
                        if (columnCount != 0)
                            sql += ", ";
                        sql += tableAlias + "." + KeyFieldNames[k];
                        columnCount++;
                    }
                }
            }

            //inject any additional columns if specified and if not already present
            if (injectColumns != null)
            {
                for (int i = 0; i < injectColumns.Length; i++)
                {
                    if (ColumnNames == null || (!ColumnNames.Contains(injectColumns[i]) && !KeyFieldNames.Contains(injectColumns[i])))
                    {
                        if (columnCount != 0)
                            sql += ", ";
                        sql += injectColumns[i];
                        columnCount++;
                    }
                }
            }

            sql += " FROM " + TableName + " " + tableAlias ;
            return sql;
        }
    }
}
