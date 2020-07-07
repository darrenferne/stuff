using System;

namespace Brady.Limits.ActionProcessing.Core
{
    public class ActionResult : IActionResult
    {
        public ActionResult(Type resultType = null)
        {
            ResultType = resultType ?? this.GetType();
        }
        public Type ResultType { get; }
    }
}
