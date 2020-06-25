using AutoMapper;
using BWF.DataServices.Support.NHibernate.Abstract;

namespace Brady.Limits.ActionProcessing.DataService
{
    public class ProcessingRequestRecordType : ChangeableRecordType<ProcessingRequest, long, ProcessingRequestBatchValidator>
    {
        public ProcessingRequestRecordType()
            : base()
        { }
        
        public override void ConfigureMapper()
        {
            Mapper.CreateMap<ProcessingRequest, ProcessingRequest>();
        }
    }
}
