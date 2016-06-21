using Brady.Trade.Domain;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Trade.Repository.Mappings
{
    public class VanillaOptionMap : JoinedSubclassMapping<VanillaOption>
    {
        public VanillaOptionMap()
        {
            Key(k =>
            {
                k.Column("OptionDetailsId");
                k.ForeignKey("fk_vanillaoption_optiondetails");
                k.NotNullable(true);
                k.OnDelete(OnDeleteAction.Cascade);
            });

            Property(vo => vo.CP);
            Property(vo => vo.CurrencyAmount);
            Property(vo => vo.ExpiryDate);
            Property(vo => vo.ExpiryMonth);
            Property(vo => vo.Model);
            Property(vo => vo.PremiumAmount);
            Property(vo => vo.PremiumCurrency);
            Property(vo => vo.PremiumDate);
            Property(vo => vo.PremiumRate);
        }
    }
}
