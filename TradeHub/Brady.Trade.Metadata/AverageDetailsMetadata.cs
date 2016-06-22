using Brady.Trade.Domain.BaseTypes;
using BWF.DataServices.Metadata.Fluent.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Trade.Metadata
{
    public class AverageDetailsMetadata : TypeMetadataProvider<AverageDetails>
    {
        public AverageDetailsMetadata()
        {
            DisplayName("Average Details");
            AutoUpdatesByDefault();

            StringProperty(x => x.AverageType)
                .DisplayName("Average Type")
                .IsHiddenInEditor();

            IntegerProperty(x => x.Id)
                .DisplayName("Id")
                .IsId()
                .IsHidden();

            BooleanProperty(x => x.IsFixedPrice)
                .DisplayName("Is Fixed?");

            NumericProperty(x => x.FixedPrice)
                .DisplayName("Fixed Price");
        }
    }
}
