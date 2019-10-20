using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchemaBrowser.Domain
{
    public static class DbTypeConversion
    {
        public static string SqlTypeToNetType(string columnType)
        {
            if (string.IsNullOrEmpty(columnType))
                return string.Empty;

            switch (columnType.ToUpper())
            {
                case "BIGINT":
                    return typeof(long).Name;
                case "INT":
                    return typeof(int).Name;
                case "DECIMAL":
                    return typeof(decimal).Name;
                case "FLOAT":
                    return typeof(double).Name;
                case "BIT":
                    return typeof(bool).Name;
                case "DATE":
                case "DATETIME":
                case "DATETIME2":
                    return typeof(DateTime).Name;
                case "DATETIMEOFFSET":
                case "TIMESTAMP WITH TIME ZONE":
                case "TIMESTAMP WITH LOCAL TIME ZONE":
                    return typeof(DateTimeOffset).Name;
                case "CHAR":
                case "VARCHAR":
                case "VARCHAR2":
                case "NCHAR":
                case "NVARCHAR":
                case "NVARCHAR2":
                    return typeof(string).Name;
                default:
                    return string.Empty;
            }
        }
        public static string NetTypeToSqlType(string netType, DatabaseType dbType, int length = -1)
        {
            if (string.IsNullOrEmpty(netType))
                return string.Empty;

            switch (netType.ToUpper())
            {
                case "INT64":
                case "LONG":
                    return dbType == DatabaseType.SqlServer ? "BIGINT" : "NUMBER(19)";
                case "INT32":
                case "INT":
                    return dbType == DatabaseType.SqlServer ? "INT" : "NUMBER(10)";
                case "DECIMAL":
                    return dbType == DatabaseType.SqlServer ? "DECIMAL" : "NUMBER";
                case "DOUBLE":
                    return dbType == DatabaseType.SqlServer ? "FLOAT" : "FLOAT";
                case "FLOAT":
                    return dbType == DatabaseType.SqlServer ? "FLOAT(24)" : "FLOAT";
                case "BOOL":
                case "BOOLEAN":
                    return dbType == DatabaseType.SqlServer ? "BIT" : "NUMBER(1)";
                case "DATETIME":
                    return dbType == DatabaseType.SqlServer ? "DATETIME" : "DATE";
                case "DATETIMEOFFSET":
                    return dbType == DatabaseType.SqlServer ? "DATETIMEOFFSET" : "TIMESTAMP WITH TIME ZONE";
                case "STRING":
                    var lengthString = length == -1 ?
                        dbType == DatabaseType.SqlServer ? "MAX" : "2000"
                        : length.ToString();
                    return dbType == DatabaseType.SqlServer ? $"NVARCHAR({length})" : $"NVARCHAR2({length})";
                default:
                    return string.Empty;
            }
        }
    }
}
