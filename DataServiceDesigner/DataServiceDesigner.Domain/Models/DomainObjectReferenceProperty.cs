using BWF.DataServices.Metadata.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace DataServiceDesigner.Domain
{
    public class DomainObjectReferenceProperty : IHaveId<long>
    {
        public virtual long Id { get; set; }
        public virtual DomainObjectReference Reference { get; set; }
        public virtual DomainObjectProperty Parent { get; set; }
        public virtual DomainObjectProperty Child { get; set; }
    }
}
