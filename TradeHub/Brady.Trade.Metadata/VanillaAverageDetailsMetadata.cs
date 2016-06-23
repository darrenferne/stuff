using Brady.Trade.Domain;
using Brady.Trade.Domain.BaseTypes;
using BWF.DataServices.Metadata.Fluent.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Trade.Metadata
{
    public class VanillaAverageDetailsMetadata : TypeMetadataProvider<VanillaAverageDetails>
    {
        public VanillaAverageDetailsMetadata()
        {
            Extends<AverageDetailsMetadata, AverageDetails>();

            DisplayName("Vanilla Average");

            DateProperty(ad => ad.StartDate)
                .DisplayName("Start Date");

            DateProperty(ad => ad.EndDate)
                .DisplayName("End Date");

            StringProperty(ad => ad.FixingIndex)
                .DisplayName("Fixing Index");

            NumericProperty(ad => ad.AdditivePremium)
                .DisplayName("Additive Premium");

            StringProperty(ad => ad.AdditivePremiumUnits)
                .DisplayName("Additive Premium Units");

            NumericProperty(ad => ad.PercentagePremium)
                .DisplayName("Percentage Premium");
        }
    }
}
