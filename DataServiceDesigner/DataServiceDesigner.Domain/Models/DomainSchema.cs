using BWF.DataServices.Metadata.Interfaces;
using System.Collections.Generic;

namespace DataServiceDesigner.Domain
{
    public class DomainSchema : IHaveId<long>
    {
        public virtual long Id { get; set; }
        public virtual DomainDataService DataService { get; set; }
        public virtual bool AddDefaultObjects { get; set; }
        public virtual string SchemaName { get; set; }
        public virtual bool IsDefault { get; set; }
        public virtual IList<DomainObject> Objects { get; set; }
        public virtual IList<DomainObjectReference> References { get; set; }
    }
}
