namespace Brady.Limits.ActionProcessing.Core
{
    public interface IActionResponse : IResponse
    {
        IActionProcessingStateChange StateChange { get; }
    }
}