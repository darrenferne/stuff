using Brady.Trade.Domain.Interfaces;
using BWF.DataServices.Metadata.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Trade.Domain.BaseTypes
{
    public abstract class AverageDetails : IHaveAssignableId<long>
    {
        public AverageDetails()
        { }

        internal AverageDetails(string averageType)
        {
            AverageType = averageType;
        }

        public virtual long Id { get; set; }
        public virtual string AverageType { get; protected internal set; }
        public virtual bool? IsFixedPrice { get; set; }
        public virtual decimal? FixedPrice { get; set; }
    }
}
