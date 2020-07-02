namespace Brady.Limits.ActionProcessing.Core
{
    public interface IStateCheckAction : IAllowedAction
    {
        IActionProcessingStateCheck InvokeCheckState(IActionRequest request);
    }
}
