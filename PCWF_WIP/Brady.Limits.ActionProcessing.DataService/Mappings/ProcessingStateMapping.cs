using NHibernate.Mapping.ByCode.Conformist;

namespace Brady.Limits.ActionProcessing.DataService
{
    public class ProcessingStateMapping : ClassMapping<ProcessingState>
    {
        public ProcessingStateMapping()
        {
            Id(p => p.Id);
            Property(p => p.ExternalId);
            Property(p => p.StateType);
            Property(p => p.StateBody);
        }
    }
}
