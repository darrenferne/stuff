using System.Collections.Generic;

namespace DataServiceDesigner.Domain
{
    public class DesignerSchemaBrowser
    {
        public DesignerConnection Connection { get; set; }
        public string SelectedSchema { get; set; }
        public IList<DesignerDomainObject> AvailableObjects { get; set; }
    }
}
