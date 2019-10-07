using BWF.DataServices.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServiceDesigner.DataService
{
    public interface ISchemaBrowserConnectionManager
    {
        void SynchConnections(IChangeSet changeSet, string token);
    }
}
