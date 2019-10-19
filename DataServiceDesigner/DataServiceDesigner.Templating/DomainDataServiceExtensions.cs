using DataServiceDesigner.Domain;
using SchemaBrowser.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServiceDesigner.Templating
{
    public static class DomainDataServiceExtensions
    {
        public static PropertyType GetKeyType(this DomainObject domainObject)
        {
            return domainObject.Properties?.SingleOrDefault(p => p.IsPartOfKey)?.PropertyType ?? PropertyType.Undefined;
        }

        public static string GetKeyProperty(this DomainObject domainObject)
        {
            return domainObject.Properties?.SingleOrDefault(p => p.IsPartOfKey)?.PropertyName ?? string.Empty;
        }

        public static bool HasCompositeKey(this DomainObject domainObject)
        {
            return domainObject.Properties.Where(p => p.IsPartOfKey).Count() > 1;
        }

        public static bool SupportsIHaveId(this DomainObject domainObject)
        {
            return domainObject.GetKeyProperty() == "Id";
        }

        public static string GetDefaultSchema(this DomainDataService dataService)
        {
            return dataService.Schemas?.FirstOrDefault(p => p.IsDefault)?.SchemaName ?? string.Empty;
        }

        public static string GetColumnType(this DomainObjectProperty property, DatabaseType dbType) 
        {
            if (string.IsNullOrEmpty(property.ColumnType))
            {
                switch (property.PropertyType)
                {
                    case PropertyType.Int32:
                        return dbType == DatabaseType.SqlServer ? "INT" : "NUMBER(10)";
                    case PropertyType.Int64:
                        return dbType == DatabaseType.SqlServer ? "BIGINT" : "NUMBER(19)";
                    case PropertyType.Float:
                        return "FLOAT(24)";
                    case PropertyType.Double:
                        return "FLOAT";
                    case PropertyType.Decimal:
                        return "DECIMAL";
                    case PropertyType.DateTime:
                        return dbType == DatabaseType.SqlServer ? "DATETIME" : "DATE";
                    case PropertyType.DateTimeOffset:
                        return dbType == DatabaseType.SqlServer ? "DATETIMEOFFSET" : "TIMESTAMP WITH TIME ZONE";
                    case PropertyType.String:
                        var propertyLength = property.Length == 0 ? 64 : property.Length;
                        return dbType == DatabaseType.SqlServer ? $"NVARCHAR({propertyLength})" : $"NVARCHAR2({propertyLength})";
                    case PropertyType.Boolean:
                        return dbType == DatabaseType.SqlServer ? "BIT" : "NUMBER(1)";
                    default:
                        return string.Empty;
                }
            }
            else
                return property.ColumnType;
        }

        public static string GetColumnName(this DomainObjectProperty property)
        {
            return string.IsNullOrEmpty(property.ColumnName) ? 
                property.PropertyName.ToLower() : 
                property.ColumnName;
        }

        public static bool RequiresValidation(this DomainObjectProperty property)
        {
            return !property.IsNullable || property.Length > 0;
        }
    }
}
