namespace Brady.Limits.ActionProcessing.Core
{
    public interface IContinuationRequest
    {
        ActionRequestDescriptor NextRequest(IActionProcessingState state);
    }
}
