using AutoMapper;
using BWF.DataServices.Support.NHibernate.Abstract;

namespace Brady.Limits.ActionProcessing.DataService
{
    public class ProcessingStateRecordType : ChangeableRecordType<ProcessingState, long, ProcessingStateBatchValidator>
    {
        public ProcessingStateRecordType()
            : base()
        { }
        
        public override void ConfigureMapper()
        {
            Mapper.CreateMap<ProcessingState, ProcessingState>();
        }
    }
}
