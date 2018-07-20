using System.Collections.Generic;

namespace SchemaBrowser.Domain
{
    public class DbConnection : IHaveAssignableId<long>
    {
        public virtual long Id { get; set; }
        public virtual string Name { get; set; }
        public virtual DbType DatabaseType { get; set; }
        public virtual string DataSource { get; set; }
        public virtual string InitialCatalog { get; set; }
        public virtual string Username { get; set; }
        public virtual string Password { get; set; }
        public virtual bool UseIntegratedSecurity { get; set; }
        public virtual string ConnectionString { get; set; }
        public virtual DbConnectionStatus Status { get; set; }
        public virtual string StatusMessage { get; set; }
    }
}
