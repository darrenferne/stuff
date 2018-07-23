using BWF.DataServices.Metadata.Interfaces;
using SchemaBrowser.Domain;
using System.Collections.Generic;

namespace DataServiceDesigner.Domain
{
    public class DomainDataService : IHaveId<long>
    {
        public virtual DbConnection DbConnection { get; set; }
        public virtual DbSchema DbSchema { get; set; }
        public virtual long Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string ConnectionString { get; set; }
        public virtual DataServiceConnection Connection { get; set; }
        public virtual string DefaultSchema { get; set; }
        public virtual IList<DomainObject> DomainObjects { get; set; }
    }
}
