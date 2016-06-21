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
    public class AverageDetailsMap : ClassMapping<AverageDetails>
    {
        public AverageDetailsMap()
        {
            Id(t => t.Id);

            Property(t => t.AverageType, m =>
            {
                m.NotNullable(true);
                m.Length(64);
            });

            Property(t => t.IsFixedPrice);
            Property(t => t.FixedPrice);
        }
    }
}
