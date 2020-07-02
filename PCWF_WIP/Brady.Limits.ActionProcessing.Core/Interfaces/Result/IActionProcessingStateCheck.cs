using System.Collections.Generic;

namespace Brady.Limits.ActionProcessing.Core
{
    public interface IActionProcessingStateCheck : IActionResult
    {
        IActionProcessingState ResultingState { get; set; }
    }
}