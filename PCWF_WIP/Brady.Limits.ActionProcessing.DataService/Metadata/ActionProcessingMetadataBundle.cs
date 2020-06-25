using BWF.DataServices.Metadata.Fluent.Abstract;

namespace Brady.Limits.ActionProcessing.DataService
{
    public class ActionProcessingMetadataBundle : TypeMetadataBundle
    {
        public ActionProcessingMetadataBundle()
            : base(ActionProcessingDataServiceConstants.DataServiceName,
                    new ProcessingRequestMetadata(),
                    new ProcessingStateMetadata())
        { }
    }
}