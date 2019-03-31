using BWF.DataServices.Metadata.Interfaces;
using System;

namespace Brady.Limits.Domain
{
    public class Selection : IHaveId<long>
    {
        public virtual long Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Filter { get; set; }
    }
}
