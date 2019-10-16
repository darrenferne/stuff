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
            return domainObject.Properties?.FirstOrDefault(p => p.IsPartOfKey)?.PropertyType ?? string.Empty;
        }

        public static string GetDefaultSchema(this DomainDataService dataService)
        {
            return dataService.Schemas?.FirstOrDefault(p => p.IsDefault)?.SchemaName ?? string.Empty;
        }
    }
}
