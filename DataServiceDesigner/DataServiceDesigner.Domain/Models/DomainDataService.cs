using BWF.DataServices.Metadata.Interfaces;
using System.Collections.Generic;

namespace DataServiceDesigner.Domain
{
    public class DomainDataService : IHaveId<long>
    {
        public virtual long Id { get; set; }
        public virtual DataServiceSolution Solution { get; set; }
        public virtual string Name { get; set; }
        public virtual DataServiceConnection Connection { get; set; }
        public virtual IList<DomainSchema> Schemas{ get; set; }
    }
}
