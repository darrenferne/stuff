using BWF.DataServices.Metadata.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServiceDesigner.Domain
{
    public class DataServiceSolution : IHaveId<long>
    {
        public DataServiceSolution()
        {
            NamespacePrefix = Defaults.NamespacePrefix;
        }

        public virtual long Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string NamespacePrefix { get; set; }
        public virtual string CompanyName { get; set; }
        public virtual string ServiceName { get; set; }
        public virtual string ServiceDisplayName { get; set; }
        public virtual string ServiceDescription { get; set; }
        public virtual int DefaultPort { get; set; }
        public virtual IList<DomainDataService> DataServices { get; set; }
    }
}
