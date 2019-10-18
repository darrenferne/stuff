using BWF.DataServices.Metadata.Interfaces;
using SchemaBrowser.Domain;

namespace DataServiceDesigner.Domain
{
    public class DataServiceConnection : IHaveId<long>
    {
        public virtual long Id { get; set; }
        public virtual string Name { get; set; }
        public virtual DatabaseType DatabaseType { get; set; }
        public virtual string DataSource { get; set; }
        public virtual string InitialCatalog { get; set; }
        public virtual string Username { get; set; }
        public virtual string Password { get; set; }
        public virtual bool UseIntegratedSecurity { get; set; }
        public virtual string ConnectionString { get; set; }
    }
}
