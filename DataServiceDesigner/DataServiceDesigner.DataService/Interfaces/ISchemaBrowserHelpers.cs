using BWF.DataServices.Domain.Interfaces;
using DataServiceDesigner.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServiceDesigner.DataService
{
    public interface ISchemaBrowserHelpers
    {
        void SynchConnections(IChangeSet changeSet, string token);
        void AddDefaultObjectsToSchema(DomainSchema domainSchema);
        void AddDefaultPropertiesToObject(DomainObject domainObject);
    }
}
