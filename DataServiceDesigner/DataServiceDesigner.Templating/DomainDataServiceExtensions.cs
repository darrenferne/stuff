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
        public static string GetKeyType(this DomainObject domainObject)
        {
            return domainObject.Properties?.SingleOrDefault(p => p.IsPartOfKey)?.PropertyType ?? string.Empty;
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
                switch (property.PropertyType.ToLower())
                {
                    case "int32":
                    case "int":
                        return "INT";
                    case "int64":
                    case "long":
                        return "BIGINT";
                    case "float":
                        return "FLOAT";
                    case "decimal":
                        return "DECIMAL";
                    case "datetime":
                        return "DATETIME";
                    case "datetimeoffset":
                        return "DATETIMEOFFSET";
                    case "string":
                        var propertyLength = property.Length == 0 ? 64 : property.Length;
                        return $"NVARCHAR({propertyLength})";
                    case "boolean":
                    case "bool":
                        return "BIT";
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
