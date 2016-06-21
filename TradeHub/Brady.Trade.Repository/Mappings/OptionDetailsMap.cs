using Brady.Trade.Domain.BaseTypes;
using Brady.Trade.Repository.CustomTypes;
using NHibernate.Mapping.ByCode.Conformist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Trade.Repository.Mappings
{
    public class OptionDetailsMap : ClassMapping<OptionDetails>
    {
        public OptionDetailsMap()
        {
            Id(t => t.Id);

            Property(t => t.OptionType, m =>
            {
                m.Length(64);
            });

            Property(t => t.OptionStatus, m =>
            {
                m.Length(64);
            });
        }
    }
}
