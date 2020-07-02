namespace Brady.Limits.ActionProcessing.Core
{
    public class ActionResponse : Response, IActionResponse
    {
        public ActionResponse(IActionRequest request, IActionResult result)
            : base(request)
        {
            ResponseName = GetType().Name;
            Result = result;
        }

        public IActionResult Result { get; }
    }
}