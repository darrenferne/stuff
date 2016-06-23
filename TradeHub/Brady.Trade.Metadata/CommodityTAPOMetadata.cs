using Brady.Trade.Domain;
using BWF.DataServices.Metadata.Fluent.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Trade.Metadata
{
    public class CommodityTAPOMetadata : TypeMetadataProvider<CommodityTAPO>
    {
        public CommodityTAPOMetadata()
        {
            Extends<CommodityTradeMetadata, Domain.CommodityTrade>();

            DisplayName("Commodity TAPO");
            
            TypeProperty(x => x.AverageDetails)
                .DisplayName("Average Details")
                .ValueFieldInEditorChoice("Id")
                .DisplayFieldInEditorChoice("FixingIndex");

            TypeProperty(x => x.OptionDetails)
                .DisplayName("Option Details")
                .ValueFieldInEditorChoice("Id")
                .DisplayFieldInEditorChoice("Model");
        }
    }
}
