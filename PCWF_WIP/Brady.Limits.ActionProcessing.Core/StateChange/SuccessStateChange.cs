namespace Brady.Limits.ActionProcessing.Core
{
    public class SuccessStateChange : StateChange
    {
        public SuccessStateChange(IActionProcessingPayload newPayload, IActionProcessingState newState, string message = null)
               : base(newPayload, newState, message)
        { }
        public SuccessStateChange(IActionProcessingPayload newPayload, IActionProcessingState newState) 
            : base(newPayload, newState)
        { }
    }
}
