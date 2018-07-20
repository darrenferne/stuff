using BWF.DataServices.Metadata.Interfaces;

namespace DataServiceDesigner.Domain
{
    public class DesignerDomainObjectProperty : IHaveId<long>
    {
        public virtual long Id { get; set; }
        public virtual string DbName { get; set; }
        public virtual string Name { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual bool IsPartOfKey { get; set; }
        public virtual bool IncludeInDefaultView { get; set; }
        public virtual long DomainObjectId { get; set; }
        public virtual DesignerDomainObject DomainObject { get; set; }
    }
}
