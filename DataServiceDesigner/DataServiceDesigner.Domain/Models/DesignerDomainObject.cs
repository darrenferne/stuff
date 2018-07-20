using BWF.DataServices.Metadata.Interfaces;
using System.Collections.Generic;

namespace DataServiceDesigner.Domain
{
    public class DesignerDomainObject : IHaveId<long>
    {
        public virtual long Id { get; set; }
        public virtual string DbSchema { get; set; }
        public virtual string DbObject { get; set; }
        public virtual string Name { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual string PluralisedDisplayName { get; set; }
        public virtual long DataServiceId { get; set; }
        public virtual DesignerDataService DataService { get; set; }
        public virtual IList<DesignerDomainObjectProperty> Properties { get; set; }
    }
}
