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
        }
    }
}
