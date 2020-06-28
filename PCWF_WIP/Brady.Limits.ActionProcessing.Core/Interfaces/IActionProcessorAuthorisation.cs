namespace Brady.Limits.ActionProcessing.Core
{
    public interface IActionProcessorAuthorisation
    {
        bool CanPerform(string userName, string actionName);
    }
}