using BWF.DataServices.Metadata.Interfaces;
using SchemaBrowser.Domain;

namespace DataServiceDesigner.Domain
{
    public class DomainObjectProperty : IHaveId<long>
    {
        public virtual DomainDataService DataService { get; set; }
        public virtual DomainObject Object { get; set; }
        public virtual long Id { get; set; }
        public virtual string DbName { get; set; }
        public virtual string Name { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual bool IsPartOfKey { get; set; }
        public virtual bool IncludeInDefaultView { get; set; }
    }
}
