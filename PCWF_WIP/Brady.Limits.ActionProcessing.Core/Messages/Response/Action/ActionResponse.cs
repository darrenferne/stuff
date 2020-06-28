namespace Brady.Limits.ActionProcessing.Core
{
    public class ActionResponse : Response, IActionResponse
    {
        public ActionResponse(IActionRequest request, IActionProcessingStateChange stateChange)
            : base(request)
        {
            ResponseName = GetType().Name;
            StateChange = stateChange;
        }

        public IActionProcessingStateChange StateChange { get; }
    }
}