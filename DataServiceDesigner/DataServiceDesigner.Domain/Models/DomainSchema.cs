using BWF.DataServices.Metadata.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServiceDesigner.Domain
{
    public class DomainSchema : IHaveId<long>
    {
        public virtual long Id { get; set; }

        public virtual string Name { get; set; }
    }
}
