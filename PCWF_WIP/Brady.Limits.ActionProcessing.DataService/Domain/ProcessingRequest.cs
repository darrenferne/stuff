using BWF.DataServices.Metadata.Interfaces;

namespace Brady.Limits.ActionProcessing.DataService
{
    public class ProcessingRequest : IHaveId<long>
    {
        public virtual long Id { get; set; }
        public virtual string RequestId { get; set; }
        public virtual string RequestType { get; set; }
        public virtual string RequestBody { get; set; }
    }
}
