using System;

namespace Brady.Limits.ActionProcessing.Core
{
    public interface IActionProcessingState : IEquatable<IActionProcessingState>
    {
        string StateName { get; }
        object ExtendedState { get; }
    }
}