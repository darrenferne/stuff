namespace Brady.Limits.ActionProcessing.Core
{
    public interface IAllowedAction
    {
        string Name { get; }

        IActionResult Invoke(IActionRequest request);
    }
}
