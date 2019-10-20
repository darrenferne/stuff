using BWF.DataServices.Metadata.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace DataServiceDesigner.Domain
{
    public class DomainObjectReference : IHaveId<long>
    {  
        public virtual long Id { get; set; }
        public virtual string ReferenceName { get; set; }
        public virtual string ConstraintName { get; set; }
        public virtual ReferenceType ReferenceType { get; set; }
        public virtual DomainObject Parent { get; set; }
        public virtual DomainObject Child { get; set; }
        public virtual IList<DomainObjectReferenceProperty> Properties { get; set; }
    }
}
