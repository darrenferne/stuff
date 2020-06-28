namespace Brady.Limits.ActionProcessing.Core
{
    public interface IRequestWithState : IActionRequest, IRequestWithContext
    {
        void SetState(IActionProcessingState state);
    }
}
