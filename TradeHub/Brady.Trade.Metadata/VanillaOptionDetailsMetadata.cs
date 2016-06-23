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

            NumericProperty(od => od.CurrencyAmount)
                .DisplayName("Currency Amount");

            StringProperty(od => od.CP)
                .DisplayName("Call/Put");

            NumericProperty(od => od.StrikePrice)
                .DisplayName("Strike Price");

            StringProperty(od => od.Model)
                .DisplayName("Model");

            DateProperty(od => od.ExpiryMonth)
                .DisplayName("Expiry Month");

            DateProperty(od => od.ExpiryDate)
                .DisplayName("Expiry Date");

            DateProperty(od => od.PremiumDate)
                .DisplayName("Premium Date");

            StringProperty(od => od.PremiumCurrency)
                .DisplayName("Premium Currency");

            NumericProperty(od => od.PremiumRate)
                .DisplayName("Premium Rate");

            NumericProperty(od => od.PremiumAmount)
                .DisplayName("Premium Amount");
        }
    }
}
