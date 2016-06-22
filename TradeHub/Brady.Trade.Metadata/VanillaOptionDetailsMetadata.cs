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
    public class VanillaOptionDetailsMetadata : TypeMetadataProvider<VanillaOptionDetails>
    {
        public VanillaOptionDetailsMetadata()
        {
            Extends<OptionDetailsMetadata, OptionDetails>();

            DisplayName("Vanilla Option");
        }
    }
}
