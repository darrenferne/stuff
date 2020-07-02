namespace Brady.Limits.ActionProcessing.Core
{
    public interface IStateChangeAction : IAllowedAction
    {
        IActionProcessingStateChange InvokeStateChange(IActionRequest request);
    }
}
