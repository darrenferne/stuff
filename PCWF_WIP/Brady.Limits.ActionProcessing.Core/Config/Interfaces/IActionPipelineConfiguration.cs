using System;
using System.Collections.Generic;

namespace Brady.Limits.ActionProcessing.Core
{
    public interface IActionPipelineConfiguration
    {
        string Name { get; }
        Dictionary<string, AllowedState> AllowedStates { get; }
        Dictionary<string, Type> ActionTypes { get; }
        IAllowedAction CreateAction(Type action);
    }
}
