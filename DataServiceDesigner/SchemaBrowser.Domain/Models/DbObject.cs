﻿using System.Collections.Generic;

namespace SchemaBrowser.Domain
{
    public class DbObject : IHaveAssignableId<long>
    {
        public virtual long Id { get; set; }
        public virtual long ConnectionId { get; set; }
        public virtual string SchemaName { get; set; }
        public virtual string Name { get; set; }
        public virtual DbObjectType ObjectType { get; set; }
        //public virtual IList<DbObjectProperty> Properties { get; set; }
    }
}