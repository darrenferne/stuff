using BWF.DataServices.Metadata.Fluent.Abstract;

namespace Brady.Limits.ActionProcessing.DataService
{
    public class ProcessingRequestMetadata : TypeMetadataProvider<ProcessingRequest>
    {
        public ProcessingRequestMetadata()
        {
            IntegerProperty(p => p.Id)
                .IsId();

            StringProperty(p => p.RequestId);
            StringProperty(p => p.RequestType);
            StringProperty(p => p.RequestBody);
        }
    }
}
