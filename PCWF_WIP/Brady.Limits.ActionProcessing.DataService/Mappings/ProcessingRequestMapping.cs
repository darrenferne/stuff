using NHibernate.Mapping.ByCode.Conformist;

namespace Brady.Limits.ActionProcessing.DataService
{
    public class ProcessingRequestMapping : ClassMapping<ProcessingRequest>
    {
        public ProcessingRequestMapping()
        {
            Id(p => p.Id);
            Property(p => p.RequestId);
            Property(p => p.RequestType);
            Property(p => p.RequestBody, m => m.Length(int.MaxValue));
        }
    }
}
