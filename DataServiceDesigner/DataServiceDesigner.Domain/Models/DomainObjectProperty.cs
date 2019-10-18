using BWF.DataServices.Metadata.Interfaces;

namespace DataServiceDesigner.Domain
{
    public class DomainObjectProperty : IHaveId<long>
    {
        public virtual long Id { get; set; }
        public virtual DomainObject Object { get; set; }
        public virtual string ColumnName { get; set; }
        public virtual string ColumnType { get; set; }
        public virtual string PropertyName { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual string PropertyType { get; set; }
        public virtual int Length { get; set; }
        public virtual bool IsNullable { get; set; }
        public virtual bool IsPartOfKey { get; set; }
        public virtual bool IncludeInDefaultView { get; set; }
    }
}
