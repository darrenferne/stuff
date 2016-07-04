using Brady.Trade.DataService.Core.Abstract;
using Brady.Trade.DataService.InMemory.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BWF.DataServices.Core.Interfaces;
using BWF.DataServices.Support.NHibernate.Abstract;
using AutoMapper;
using BWF.DataServices.Metadata.Interfaces;
using BWF.DataServices.Metadata.Attributes.Actions;
using Brady.Trade.Domain.BaseTypes;
using Brady.Trade.Domain;
using Brady.Trade.Repository;
using Brady.Trade.Domain.Interfaces;

namespace Brady.Trade.DataService.RecordTypes
{
    [CreateAction("Option Details", IncludeSubTypes = new string[] { "VanillaOptionDetails" },
                                    SubTypeDisplayNames = new string[] { "Vanilla Opton" })]
    [EditAction("Option Details")]
    public class OptionDetailsRecordType : OptionDetailsRecordType<OptionDetails>
    {
        public OptionDetailsRecordType(IMetadataProvider metadataProvider, IInMemoryTradeDataServiceRepository repository)
            : base(metadataProvider, repository)
        { }
        
        public override void ConfigureMapper()
        {
            Mapper.CreateMap<OptionDetails, OptionDetails>()
                .Include<VanillaOptionDetails, VanillaOptionDetails>();
        }
    }

    public class OptionDetailsRecordType<T_record> : InMemoryRecordType<T_record, OptionDetailsValidator<T_record>>
        where T_record : class, IHaveAssignableId<long>
    {
        public OptionDetailsRecordType(IMetadataProvider metadataProvider, IInMemoryTradeDataServiceRepository repository)
            : base(metadataProvider, repository)
        { }

        public override void ConfigureMapper()
        {
            Mapper.CreateMap<T_record, T_record>();
        }
    }
}
