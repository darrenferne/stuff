namespace Brady.Limits.ActionProcessing.Core
{
    public interface IStatePersistentRequest : IActionRequest
    {
        void SetState(IActionProcessingState state);
    }
}
