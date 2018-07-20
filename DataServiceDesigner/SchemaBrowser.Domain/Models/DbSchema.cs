using System.Collections.Generic;

namespace SchemaBrowser.Domain
{
    public class DbSchema : IHaveAssignableId<long>
    {
        public long Id { get; set; }

        public virtual long ConnectionId { get; set; }
        public string Name { get; set; }
        //public virtual IList<DbObject> Objects { get; set; }
    }
}
