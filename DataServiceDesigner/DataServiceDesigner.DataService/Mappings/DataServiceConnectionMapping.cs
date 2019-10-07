using DataServiceDesigner.Domain;
using NHibernate.Mapping.ByCode.Conformist;

namespace DataServiceDesigner.DataService
{
    public class DataServiceConnectionMap : ClassMapping<DataServiceConnection>
    {
        public DataServiceConnectionMap()
        {
            Table("connection");

            Id(x => x.Id);

            Property(x => x.Name);
            Property(x => x.DatabaseType);
            Property(x => x.DataSource);
            Property(x => x.InitialCatalog);
            Property(x => x.Username);
            Property(x => x.Password);
            Property(x => x.UseIntegratedSecurity);
            Property(x => x.ConnectionString);
        }
    }
}
