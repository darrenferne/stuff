using Brady.Trade.Domain;
using BWF.DataServices.Metadata.Fluent.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Trade.Metadata
{
    public class CommodityOptionMetadata : TypeMetadataProvider<CommodityOption>
    {
        public CommodityOptionMetadata()
        {
            Extends<CommodityTradeMetadata, Domain.CommodityTrade>();

            DisplayName("Commodity Option");
            
            TypeProperty(x => x.OptionDetails)
                .DisplayName("Option Details")
                .ValueFieldInEditorChoice("Id")
                .DisplayFieldInEditorChoice("Model");
        }
    }
}
