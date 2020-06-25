using BWF.DataServices.Metadata.Interfaces;

namespace Brady.Limits.ActionProcessing.DataService
{
    public class ProcessingState : IHaveId<long>
    {
        public virtual long Id { get; set; }
        public virtual string ExternalId { get; set; }
        public virtual string StateType { get; set; }
        public virtual string StateBody { get; set; }
    }
}
