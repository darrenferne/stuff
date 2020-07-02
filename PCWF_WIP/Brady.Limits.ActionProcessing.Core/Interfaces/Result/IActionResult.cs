using System;
using System.Collections.Generic;

namespace Brady.Limits.ActionProcessing.Core
{
    public interface IActionResult
    {
        Type ResultType { get; }
    }
}