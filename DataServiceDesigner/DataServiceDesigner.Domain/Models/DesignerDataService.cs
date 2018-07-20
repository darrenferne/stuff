using BWF.DataServices.Metadata.Interfaces;
using SchemaBrowser.Domain;
using System.Collections.Generic;

namespace DataServiceDesigner.Domain
{
    public class DesignerDataService : IHaveId<long>
    {
        public virtual long Id { get; set; }
        public virtual string Name { get; set; }
        public virtual long ConnectionId { get; set; }
        public virtual DbConnection Connection { get; set; }
        public virtual string DefaultSchema { get; set; }
        public virtual IList<DesignerDomainObject> DomainObjects { get; set; }
    }
}
