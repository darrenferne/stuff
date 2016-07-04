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
using Brady.Trade.Domain.BaseTypes;
using BWF.DataServices.Metadata.Attributes.Actions;
using Brady.Trade.Domain;
using Brady.Trade.Repository;
using Brady.Trade.Domain.Interfaces;

namespace Brady.Trade.DataService.RecordTypes
{
    [CreateAction("Average Details", IncludeSubTypes = new string[] { "VanillaAverageDetails" },
                                     SubTypeDisplayNames = new string[] { "Vanilla Average" })]
    [EditAction("Average Details")]
    public class AverageDetailsRecordType : AverageDetailsRecordType<AverageDetails>
    {
        public AverageDetailsRecordType(IMetadataProvider metadataProvider, IInMemoryTradeDataServiceRepository repository)
            : base(metadataProvider, repository)
        { }

        public override void ConfigureMapper()
        {
            Mapper.CreateMap<AverageDetails, AverageDetails>()
                .Include<VanillaAverageDetails, VanillaAverageDetails>();
        }
    }
        
    public class AverageDetailsRecordType<T_record> : InMemoryRecordType<T_record, AverageDetailsValidator<T_record>>
        where T_record : class, IHaveAssignableId<long>
    {
        public AverageDetailsRecordType(IMetadataProvider metadataProvider, IInMemoryTradeDataServiceRepository repository)
            : base(metadataProvider, repository)
        { }
    }
}
