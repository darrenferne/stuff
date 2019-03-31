﻿namespace SchemaBrowser.Domain
{
    public class DbObjectProperty : IHaveAssignableId<long>
    {
        public virtual long Id { get; set; }
        public virtual long ConnectionId { get; set; }
        public virtual string SchemaName { get; set; }
        public virtual string ObjectName { get; set; }
        public virtual string Name { get; set; }
    }
}