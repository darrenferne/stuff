using System;

namespace Brady.Limits.ActionProcessing.Core
{
    public interface IActionProcessingState : IEquatable<IActionProcessingState>
    {
        string CurrentState { get; }
        object ExtendedState { get; }
    }
}