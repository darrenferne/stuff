using System.Collections.Generic;

namespace SchemaBrowser.Domain
{
    public class DbSchema : IHaveAssignableId<long>
    {
        public virtual long Id { get; set; }
        public virtual DbConnection Connection { get; set; }
        public virtual string Name { get; set; }
    }
}
