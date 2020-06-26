namespace Brady.Limits.ActionProcessing.Core
{
    public interface IActionProcessorUser
    {
        string UserName { get; }
        bool CanPerform(string actionName);
    }
}