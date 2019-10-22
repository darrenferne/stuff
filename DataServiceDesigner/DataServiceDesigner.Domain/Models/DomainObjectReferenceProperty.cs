using BWF.DataServices.Metadata.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace DataServiceDesigner.Domain
{
    public class DomainObjectReferenceProperty : IHaveId<long>
    {
        public virtual long Id { get; set; }
        public virtual long ReferenceId { get; set; }
        public virtual DomainObjectReference Reference { get; set; }
        public virtual DomainObjectProperty ParentProperty { get; set; }
        public virtual long ParentPropertyId { get; set; }
        public virtual DomainObjectProperty ChildProperty { get; set; }
        public virtual long ChildPropertyId { get; set; }
    }
}
