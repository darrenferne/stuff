using System.Collections.Generic;
using System.Linq;

namespace Brady.Limits.ActionProcessing.Core
{
    public class StateChange : IActionProcessingStateChange
    {
        public StateChange(IActionProcessingPayload newPayload, IActionProcessingState newState, string message)
            : this(newPayload, newState, Enumerable.Repeat(message, 1))
        { }

        public StateChange(IActionProcessingPayload newPayload, IActionProcessingState newState, IEnumerable<string> messages = null)
        {
            NewPayload = newPayload;
            NewState = newState;
            Messages = new List<string>(messages ?? Enumerable.Empty<string>());
        }

        public IActionProcessingPayload NewPayload { get; set; }
        public IActionProcessingState NewState { get; set; }
        public IEnumerable<string> Messages { get; set; }
    }
}
