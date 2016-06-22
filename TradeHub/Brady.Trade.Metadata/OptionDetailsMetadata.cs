using Brady.Trade.Domain.BaseTypes;
using BWF.DataServices.Metadata.Fluent.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Trade.Metadata
{
    public class OptionDetailsMetadata : TypeMetadataProvider<OptionDetails>
    {
        public OptionDetailsMetadata()
        {
            DisplayName("Option Details");
            AutoUpdatesByDefault();

            StringProperty(x => x.OptionType)
                .DisplayName("Option Type")
                .IsHiddenInEditor();

            IntegerProperty(x => x.Id)
                .DisplayName("Id")
                .IsId()
                .IsHidden();

            StringProperty(x => x.OptionStatus)
                .DisplayName("Option Status");
        }
    }
}
