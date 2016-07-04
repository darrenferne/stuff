using Brady.Trade.Domain.Interfaces;
using BWF.DataServices.Metadata.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Trade.Domain.BaseTypes
{
    public abstract class OptionDetails : IHaveAssignableId<long>
    {
        public OptionDetails()
        { }

        internal OptionDetails(string optionType)
        {
            OptionType = optionType;
        }

        public virtual long Id { get; set; }
        public virtual string OptionType { get; protected internal set; }
        public virtual string OptionStatus { get; set; }
    }
}
