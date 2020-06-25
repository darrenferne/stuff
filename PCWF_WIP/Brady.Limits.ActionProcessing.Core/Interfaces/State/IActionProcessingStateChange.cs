using System.Collections.Generic;

namespace Brady.Limits.ActionProcessing.Core
{
    public interface IActionProcessingStateChange
    {
        IEnumerable<string> Messages { get; set; }
        IActionProcessingPayload NewPayload { get; set; }
        IActionProcessingState NewState { get; set; }
    }
}