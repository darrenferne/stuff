using BWF.DataServices.Metadata.Fluent.Abstract;

namespace Brady.Limits.ProvisionalContract.Domain.Metadata
{
    public class ProvisionalContractMetadata : TypeMetadataProvider<ProvisionalContract>
    {
        public ProvisionalContractMetadata()
        {
            DisplayName("Provisional Contract");

            AutoUpdatesByDefault();

            IntegerProperty(p => p.Id)
                .IsId()
                .IsHidden()
                .IsHiddenInEditor();

            StringProperty(p => p.ContractId)
                .DisplayName("Contract Id");

            StringProperty(p => p.ClientNumber)
                .DisplayName("Client Number");

            StringProperty(p => p.ClientName)
                .DisplayName("Client Name");

            StringProperty(p => p.Product)
                .DisplayName("Product");

            NumericProperty(p => p.Quantity)
                .DisplayName("Quantity");

            StringProperty(p => p.QuantityUnit)
                .DisplayName("QuantityUnits");

            NumericProperty(p => p.Premium)
                .DisplayName("Premium");

            IntegerProperty(p => p.Status)
                .DisplayName("Status");

            ViewDefaults()
                .Property(p => p.ContractId)
                .Property(p => p.ClientNumber)
                .Property(p => p.ClientName)
                .Property(p => p.Product)
                .Property(p => p.Quantity)
                .Property(p => p.QuantityUnit)
                .Property(p => p.Premium)
                .Property(p => p.Status)
                .OrderBy(p => p.ClientNumber);
        }
    }
}