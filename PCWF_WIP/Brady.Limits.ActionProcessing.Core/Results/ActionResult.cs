using System;
using System.Collections.Generic;
using System.Linq;

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
