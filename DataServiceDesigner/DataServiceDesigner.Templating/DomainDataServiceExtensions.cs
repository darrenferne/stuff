using DataServiceDesigner.Domain;
using System;
using System.Collections.Generic;
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
    }
}
