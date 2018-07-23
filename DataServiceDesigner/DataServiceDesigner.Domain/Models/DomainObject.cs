using BWF.DataServices.Metadata.Interfaces;
using SchemaBrowser.Domain;
using System.Collections.Generic;

namespace DataServiceDesigner.Domain
{
    public class DomainObject : IHaveId<long>
    {
        public virtual DomainDataService DataService { get; set; }
        public virtual DbSchema DbSchema { get; set; }
        public virtual DbObject DbObject { get; set; }
        public virtual long Id { get; set; }
        public virtual string DbSchemaName { get; set; }
        public virtual string DbObjectName { get; set; }
        public virtual string Name { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual string PluralisedDisplayName { get; set; }
        public virtual IList<DomainObjectProperty> ObjectProperties { get; set; }
    }
}
