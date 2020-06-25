using System.Collections.Generic;

namespace Brady.Limits.ActionProcessing.Core
{
    public class FailureStateChange : StateChange
    {
        public FailureStateChange(IActionProcessingPayload newPayload, IActionProcessingState newState, string message = null)
            : base(newPayload, newState, message)
        { }
        public FailureStateChange(IActionProcessingPayload newPayload, IActionProcessingState newState, IEnumerable<string> messages = null) 
            : base(newPayload, newState, messages)
        { }
    }
}
