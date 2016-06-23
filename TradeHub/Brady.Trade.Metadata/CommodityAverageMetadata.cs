using Brady.Trade.Domain;
using BWF.DataServices.Metadata.Fluent.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Trade.Metadata
{
    public class CommodityAverageMetadata : TypeMetadataProvider<CommodityAverage>
    {
        public CommodityAverageMetadata()
        {
            Extends<CommodityTradeMetadata, Domain.CommodityTrade>();

            DisplayName("Commodity Average");
            
            TypeProperty(x => x.AverageDetails)
                .DisplayName("Average Details")
                .ValueFieldInEditorChoice("Id")
                .DisplayFieldInEditorChoice("FixingIndex");

        }
    }
}
