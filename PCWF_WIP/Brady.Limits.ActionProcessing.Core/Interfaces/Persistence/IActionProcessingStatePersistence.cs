namespace Brady.Limits.ActionProcessing.Core
{
    public interface IActionProcessingStatePersistence 
    {
        IActionProcessingState GetInitialState(IActionRequest request);
        IActionProcessingState GetCurrentState(IActionRequest request);
        void SetCurrentState(IActionRequest request, IActionProcessingState newState);
    }
}
