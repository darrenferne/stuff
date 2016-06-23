using Brady.Trade.Repository.CustomTypes;
using NHibernate.Mapping.ByCode.Conformist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Trade.Repository.Mappings
{
    public class TradeMap : ClassMapping<Domain.Trade>
    {
        public TradeMap()
        {
            Id(t => t.Id, m => m.Column("Id"));

            Property(t => t.ContractCode, m =>
            {
                m.Length(64);
            });

            Property(t => t.Counterparty, m =>
            {
                m.Length(64);
            });

            Property(t => t.EffectiveDate);

            Property(t => t.EnteredBy, m =>
            {
                m.Length(64);
            });

            Property(t => t.EnteredOn);

            Property(t => t.Entity, m =>
            {
                m.Length(64);
            });

            Property(t => t.IsLive, m =>
            {
                m.Type<NumericTrueFalse>();
            });

            Property(t => t.MarketCode, m =>
            {
                m.Length(64);
            });

            Property(t => t.Portfolio, m =>
            {
                m.Length(64);
            });

            Property(t => t.SystemCode, m =>
            {
                m.NotNullable(true);
                m.Length(64);
            });

            Property(t => t.SystemId, m =>
            {
                m.NotNullable(true);
                m.Length(64);
            });

            Property(t => t.TradedBy, m =>
            {
                m.Length(64);
            });

            Property(t => t.TradedOn);

            Property(t => t.TradeType, m =>
            {
                m.NotNullable(true);
                m.Length(64);
            });
        }
    }
}
