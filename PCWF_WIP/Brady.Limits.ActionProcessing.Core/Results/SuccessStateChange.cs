namespace Brady.Limits.ActionProcessing.Core
{
    public class SuccessStateChange : StateChangeResult
    {
        public SuccessStateChange(IActionRequestPayload newPayload, IActionProcessingState newState, string message = null)
               : base(newPayload, newState, message)
        { }
        public SuccessStateChange(IActionRequestPayload newPayload, IActionProcessingState newState) 
            : base(newPayload, newState)
        { }
    }
}
