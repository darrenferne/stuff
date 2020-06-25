using System;

namespace Brady.Limits.ActionProcessing.Core
{
    public interface IActionFactory
    {
        Type ResolveType(string name);
        IAllowedAction CreateAction(string name);
        IAllowedAction CreateAction(Type type);
    }
}