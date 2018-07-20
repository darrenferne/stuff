using DataServiceDesigner.Domain;
using System.Collections.Generic;

namespace DataServiceDesigner.DataService
{
    public interface ISchemaRepository
    {
        void FetchSchema(DesignerConnection connection, bool update = false);
        IList<string> GetAvailableSchemas(DesignerConnection connection);
        IList<DesignerDomainObject> GetObjects(DesignerConnection connection, string owner);
        IList<DesignerDomainObjectProperty> GetObjectProperties(DesignerConnection connection, string owner, string objectName);
    }
}