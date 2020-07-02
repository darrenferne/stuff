using System.Collections.Generic;
using System.Linq;

namespace Brady.Limits.ActionProcessing.Core
{
    public static class ActionResultExtensions
    {
        public static IActionProcessingStateChange GetStateChange(this IActionResult result) => result as IActionProcessingStateChange;
        public static IActionProcessingStateChange GetStateChange(this IResponse response) => (response as IActionResponse)?.Result as IActionProcessingStateChange;
    }
}
