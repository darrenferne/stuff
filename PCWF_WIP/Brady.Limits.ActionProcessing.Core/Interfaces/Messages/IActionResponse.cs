namespace Brady.Limits.ActionProcessing.Core
{
    public interface IActionResponse : IResponse
    {
        IActionResult Result { get; }
    }
}