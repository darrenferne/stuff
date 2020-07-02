using System.Collections.Generic;

namespace Brady.Limits.ActionProcessing.Core
{
    public interface IActionProcessingStateChange : IActionResult
    {
        IEnumerable<string> Messages { get; set; }
        IActionRequestPayload NewPayload { get; set; }
        IActionProcessingState NewState { get; set; }
    }
}