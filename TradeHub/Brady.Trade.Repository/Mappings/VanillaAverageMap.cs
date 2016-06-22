using Brady.Trade.Domain;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Trade.Repository.Mappings
{
    public class VanillaAverageMap : JoinedSubclassMapping<VanillaAverageDetails>
    {
        public VanillaAverageMap()
        {
            Key(k =>
            {
                k.Column("AverageDetailsId");
                k.ForeignKey("fk_vanillaaverage_averagedetails");
                k.NotNullable(true);
                k.OnDelete(OnDeleteAction.Cascade);
            });

            Property(va => va.AdditivePremium);
            Property(va => va.AdditivePremiumUnits);
            Property(va => va.AverageType);
            Property(va => va.EndDate);
            Property(va => va.FixingIndex);
            Property(va => va.PercentagePremium);
            Property(va => va.StartDate);
        }
    }
}
