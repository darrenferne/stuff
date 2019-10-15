namespace SchemaBrowser.Domain
{
    public class DbObjectProperty : IHaveAssignableId<long>
    {
        public virtual long Id { get; set; }
        public virtual DbConnection Connection { get; set; }
        public virtual string SchemaName { get; set; }
        public virtual string ObjectName { get; set; }
        public virtual string Name { get; set; }
        public virtual string ColumnType { get; set; }
        public virtual int ColumnLength { get; set; }
        public virtual bool IsNullable { get; set; }
        public virtual string NetType { get; set; }
    }
}
