using System.Collections.Generic;

namespace Brady.Limits.ActionProcessing.Core
{
    public interface IAllowedState
    {
        IDictionary<string, IAllowedAction> AllowedActions { get; }
        string Name { get; }
    }
}