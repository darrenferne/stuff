using BWF.DataServices.Metadata.Fluent.Abstract;

namespace Brady.Limits.ActionProcessing.DataService
{
    public class ProcessingStateMetadata : TypeMetadataProvider<ProcessingState>
    {
        public ProcessingStateMetadata()
        {
            IntegerProperty(p => p.Id)
                .IsId();

            StringProperty(p => p.ExternalId);
            StringProperty(p => p.StateType);
            StringProperty(p => p.StateBody);
        }
    }
}
