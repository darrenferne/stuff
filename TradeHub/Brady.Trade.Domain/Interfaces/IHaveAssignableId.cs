using BWF.DataServices.Metadata.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Trade.Domain.Interfaces
{
    public interface IHaveAssignableId<Tid> : IHaveId<long>
    {
        new Tid Id { get; set; }
    }
}
