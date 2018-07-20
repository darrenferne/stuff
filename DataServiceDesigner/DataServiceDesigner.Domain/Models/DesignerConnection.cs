using BWF.DataServices.Metadata.Interfaces;

namespace DataServiceDesigner.Domain
{
    public class DesignerConnection : IHaveId<long>
    {
        public virtual long Id { get; set; }
        public virtual string Name { get; set; }
        public virtual DesignerDatabaseType DatabaseType { get; set; }
        public virtual string DataSource { get; set; }
        public virtual string InitialCatalog { get; set; }
        public virtual string Username { get; set; }
        public virtual string Password { get; set; }
        public virtual bool UseIntegratedSecurity { get; set; }
        public virtual string ConnectionString { get; set; }
    }
}
