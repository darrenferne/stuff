using DataServiceDesigner.Domain;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace DataServiceDesigner.DataService
{
    public class DomainSchemaMap : ClassMapping<DomainSchema>
    {
        public DomainSchemaMap()
       {
            Table("domainschema");

            Id(x => x.Id);

            Property(x => x.Name, m => m.Column("Name"));
        }
    }
}
