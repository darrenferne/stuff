using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Trade.Domain.BaseTypes
{
    public class OptionDetails
    {
        public OptionDetails(string optionType)
        {
            OptionType = optionType;
        }

        public virtual int Id { get; set; }
        public virtual string OptionType { get; set; }
        public virtual string OptionStatus { get; set; }
    }
}
