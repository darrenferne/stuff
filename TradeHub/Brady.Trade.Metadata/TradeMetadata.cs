using BWF.DataServices.Metadata.Fluent.Abstract;
using Brady.Trade.Domain;

namespace Brady.Trade.Metadata
{
    public class TradeMetadata : TypeMetadataProvider<Domain.Trade>
    {
        public TradeMetadata()
        {
            DisplayName("Trade");
            AutoUpdatesByDefault();

            IdentificationSummaryFields()
                .Property(x => x.SystemCode)
                .Property(x => x.SystemId);

            StringProperty(x => x.TradeType)
                .DisplayName("Trade Type")
                .IsHiddenInEditor();

            StringProperty(x => x.ContractCode)
                .DisplayName("Contract Code");

            StringProperty(x => x.MarketCode)
                .DisplayName("Market Code");

            StringProperty(x => x.SystemCode)
                .DisplayName("System Code")
                .IsId();

            StringProperty(x => x.SystemId)
                .DisplayName("System Id")
                .IsId();

            BooleanProperty(x => x.IsLive)
                .DisplayName("IsLive?");

            StringProperty(x => x.Entity)
                .DisplayName("Entity");

            StringProperty(x => x.Counterparty)
                .DisplayName("Counterparty");

            StringProperty(x => x.Portfolio)
                .DisplayName("Portfolio");

            StringProperty(x => x.TradedBy)
                .DisplayName("Traded By");

            DateProperty(x => x.TradedOn)
                .DisplayName("Traded On");

            StringProperty(x => x.EnteredBy)
                .DisplayName("Entered By");

            DateProperty(x => x.EnteredOn)
                .DisplayName("Entered On");

            StringProperty(x => x.UpdatedBy)
                .DisplayName("Updated By");

            DateProperty(x => x.UpdateOn)
                .DisplayName("Update On");

            DateProperty(x => x.EffectiveDate)
                .DisplayName("Effective Date");

            ViewDefaults()
                .Property(x => x.Id)
                .Property(x => x.TradeType)
                .Property(x => x.ContractCode)
                .Property(x => x.MarketCode)
                .Property(x => x.SystemCode)
                .Property(x => x.SystemId)
                .Property(x => x.IsLive)
                .Property(x => x.Entity)
                .Property(x => x.Counterparty)
                .Property(x => x.Portfolio)
                .Property(x => x.TradedBy)
                .Property(x => x.TradedOn)
                .Property(x => x.EnteredBy)
                .Property(x => x.EnteredOn)
                .Property(x => x.UpdatedBy)
                .Property(x => x.UpdateOn)
                .Property(x => x.EffectiveDate);
        }
    }
}
