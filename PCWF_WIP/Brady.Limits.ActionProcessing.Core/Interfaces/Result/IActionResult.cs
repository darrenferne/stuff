using System;

namespace Brady.Limits.ActionProcessing.Core
{
    public interface IActionResult
    {
        Type ResultType { get; }
    }
}