using Brady.Trade.DataService.Core.Abstract;
using Brady.Trade.DataService.Database.Validation;
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

namespace Brady.Trade.DataService.RecordTypes
{
    [CreateAction("Average Details", IncludeSubTypes = new string[] { "VanillaAverageDetails" },
                                     SubTypeDisplayNames = new string[] { "Vanilla Average" })]
    [EditAction("Average Details")]
    public class AverageDetailsRecordType : AverageDetailsRecordType<AverageDetails>
    {
        public AverageDetailsRecordType(IMetadataProvider metadataProvider)
            : base(metadataProvider)
        { }

        public override void ConfigureMapper()
        {
            Mapper.CreateMap<AverageDetails, AverageDetails>()
                .Include<VanillaAverageDetails, VanillaAverageDetails>();
        }
    }
        
    public class AverageDetailsRecordType<T> : ChangeableRecordType<T, long, AverageDetailsValidator<T>, AverageDetailsDeleteValidator<T>>
        where T : class, IHaveId<long>
    {
        IMetadataProvider _metadataProvider;

        public AverageDetailsRecordType(IMetadataProvider metadataProvider)
            : base()
        {
            _metadataProvider = metadataProvider;
        }

        public override void ConfigureMapper()
        {
            Mapper.CreateMap<T, T>();
        }
    }
}
