namespace Brady.Limits.ActionProcessing.Core
{
    public interface IAllowedAction
    {
        string Name { get; }

        IActionProcessingStateChange Invoke(IActionRequest request);
    }
}
