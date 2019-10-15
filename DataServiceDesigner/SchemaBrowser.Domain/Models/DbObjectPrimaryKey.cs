using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchemaBrowser.Domain
{
    public class DbObjectPrimaryKey : IHaveAssignableId<long>
    {
        public DbObjectPrimaryKey()
        {
            Columns = new List<string>();
        }
        public virtual long Id { get; set; }
        public virtual DbConnection Connection { get; set; }
        public string SchemaName { get; set; }
        public string TableName { get; set; }
        public string IndexName { get; set; }
        public List<string> Columns { get; set; }
        public string ColumnSummary => string.Join(";", Columns);
    }
}
