using BWF.DataServices.Metadata.Interfaces;
using System;
using System.Collections.Generic;

namespace Brady.Limits.Domain
{
    public class Workflow : IHaveId<long>
    {
        public virtual long Id { get; set; }
        public virtual string Name { get; set; }
        public virtual IList<WorkflowLevel> Levels { get; set; }
    }
}
