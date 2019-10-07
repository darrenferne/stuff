using BWF.DataServices.Metadata.Interfaces;
using SchemaBrowser.Domain;
using System.Collections.Generic;

namespace DataServiceDesigner.Domain
{
    public class DomainObject : IHaveId<long>
    {   public virtual long Id { get; set; }
        public virtual DomainDataService DataService { get; set; }
        public virtual DomainSchema Schema{ get; set; }
        public virtual string DbName { get; set; }
        public virtual string Name { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual string PluralisedDisplayName { get; set; }
        public virtual IList<DomainObjectProperty> ObjectProperties { get; set; }
    }
}
