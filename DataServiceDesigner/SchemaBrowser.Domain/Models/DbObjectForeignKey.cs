using System.Collections.Generic;

namespace SchemaBrowser.Domain
{
    public class DbObjectForeignKey : IHaveAssignableId<long>
    {
        public DbObjectForeignKey()
        {
            Columns = new List<string>();
        }
        public virtual long Id { get; set; }
        public virtual DbConnection Connection { get; set; }
        public string SchemaName { get; set; }
        public string TableName { get; set; }
        public string ConstraintName { get; set; }
        public List<string> Columns { get; set; }
        public DbObjectIndex ReferencedIndex { get; set; }
        public string ColumnSummary => string.Join(";", Columns);
        public string ReferencedIndexSummary => $"{ReferencedIndex.IndexName} - {ReferencedIndex.SchemaName}.{ReferencedIndex.TableName}({ReferencedIndex.ColumnSummary})";
    }
}
