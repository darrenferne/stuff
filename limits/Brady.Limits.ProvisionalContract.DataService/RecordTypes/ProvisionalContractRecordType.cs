using AutoMapper;
using BWF.DataServices.Core.Interfaces;
using BWF.DataServices.Metadata.Attributes.Actions;
using BWF.DataServices.Support.NHibernate.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace Brady.Limits.ProvisionalContract.DataService.RecordTypes
{
    [CreateAction("Physical Contract")]
    [EditAction("Physical Contract")]
    [DeleteAction("Physical Contract")]
    public class ProvisionalContractRecordType : ChangeableRecordType<Domain.ProvisionalContract, long, ProvisionalContractBatchValidator>
    {

        public ProvisionalContractRecordType()
            : base()
        { }

        public override void ConfigureMapper()
        {
            Mapper.CreateMap<Domain.ProvisionalContract, Domain.ProvisionalContract>();
        }
    }
}
