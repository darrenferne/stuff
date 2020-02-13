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
    public static class ModelExtensions
    {
        public static string GetNamespace(this DomainDataService dataService)
        {
            return dataService.Solution?.GetNamespace();
        }
        public static string GetNamespace(this DataServiceSolution solution)
        {
            return string.IsNullOrEmpty(solution.NamespacePrefix) ? Defaults.NamespacePrefix : solution.NamespacePrefix;
        }

        public static string ToCSharpType(this PropertyType propertyType)
        {
            switch (propertyType)
            {
                case PropertyType.Int32:
                    return "int";
                case PropertyType.Int64:
                    return "long";
                case PropertyType.Boolean:
                    return "bool";
                case PropertyType.DateTime:
                case PropertyType.DateTimeOffset:
                case PropertyType.Guid:
                    return propertyType.ToString();
                case PropertyType.String:
                case PropertyType.Decimal:
                case PropertyType.Double:
                case PropertyType.Float:
                    return propertyType.ToString().ToLower();
                default:
                    return string.Empty;
            }
        }
        public static string GetKeyType(this DomainObject domainObject)
        {
            return (domainObject.Properties?.SingleOrDefault(p => p.IsPartOfKey)?.PropertyType ?? PropertyType.Undefined).ToCSharpType();
        }

        public static string GetKeyProperty(this DomainObject domainObject)
        {
            if (domainObject.HasCompositeKey())
                return string.Empty;

            return domainObject.Properties?.SingleOrDefault(p => p.IsPartOfKey)?.PropertyName  ?? string.Empty;
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
                return DbTypeConversion.NetTypeToSqlType(property.PropertyType.ToString(), dbType, property.Length);
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
