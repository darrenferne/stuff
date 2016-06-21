using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Trade.Domain.BaseTypes
{
    public class AverageDetails
    {
        public AverageDetails(string averageType)
        {
            AverageType = averageType;
        }
        public virtual int Id { get; set; }
        public virtual string AverageType { get; set; }
        public virtual bool? IsFixedPrice { get; set; }
        public virtual decimal? FixedPrice { get; set; }
    }
}
