using Brady.Trade.DataService.Core.Abstract;
using Brady.Trade.DataService.Core.Validators;
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

namespace Brady.Trade.DataService.RecordTypes
{
    [CreateAction("Option Details", IncludeSubTypes = new string[] { "VanillaOptionDetails" },
                                    SubTypeDisplayNames = new string[] { "Vanilla Opton" })]
    [EditAction("Option Details")]
    public class OptionDetailsRecordType : OptionDetailsRecordType<OptionDetails>
    {
        public OptionDetailsRecordType(IMetadataProvider metadataProvider)
            : base(metadataProvider)
        { }
        
        public override void ConfigureMapper()
        {
            Mapper.CreateMap<OptionDetails, OptionDetails>()
                .Include<VanillaOptionDetails, VanillaOptionDetails>();
        }
    }

    public class OptionDetailsRecordType<T> : ChangeableRecordType<T, long, OptionDetailsValidator<T>, OptionDetailsDeleteValidator<T>>
        where T : class, IHaveId<long>
    {
        IMetadataProvider _metadataProvider;

        public OptionDetailsRecordType(IMetadataProvider metadataProvider)
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
