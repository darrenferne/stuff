using AutoMapper;
using Brady.Trade.Domain;
using BWF.DataServices.Core.Interfaces;
using BWF.DataServices.Support.NHibernate.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Trade.DataService.Abstract
{
    public class ChangeableTradeRecordType<Titem, Tvalidator, TdeleteValidator>
        : ChangeableRecordType<Titem, string, Tvalidator, TdeleteValidator>
        where Titem : Domain.Trade
        where Tvalidator : FluentValidation.AbstractValidator<Titem>, BWF.DataServices.Support.NHibernate.Interfaces.IRequireCrudingDataServiceRepository, new()
        where TdeleteValidator : FluentValidation.AbstractValidator<Titem>, BWF.DataServices.Support.NHibernate.Interfaces.IRequireCrudingDataServiceRepository, new()
    {
        protected string _typeName = typeof(Titem).Name;
        protected IMetadataProvider _metadataProvider;

        public ChangeableTradeRecordType(IMetadataProvider metadataProvider)
            : base()
        {
            _metadataProvider = metadataProvider;
        }

        public override string TypeName
        {
            get { return _typeName; }
        }

        public override void ConfigureMapper()
        {
            Mapper.CreateMap<Titem, Titem>();
        }
    }
}
