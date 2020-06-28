namespace Brady.Limits.ActionProcessing.Core
{
    public interface IActionProcessorAuthorisation
    {
        string SystemUserName { get; }
        bool CanPerform(string userName, string actionName);
    }
}