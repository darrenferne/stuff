using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Brady.Limits.ActionProcessing.Core
{
    public class AllowedState : IAllowedState
    {
        public AllowedState(string name)
            : this(name, new List<IAllowedAction>())
        { }

        public AllowedState(string name, params IAllowedAction[] allowedActions)
            : this(name, new List<IAllowedAction>(allowedActions ?? Enumerable.Empty<IAllowedAction>()))
        { }

        public AllowedState(string name, params string[] allowedActions)
            : this(name, new List<string>(allowedActions ?? Enumerable.Empty<string>()))
        { }

        public AllowedState(string name, IList<IAllowedAction> allowedActions)
            : this(name, allowedActions.ToDictionary(a => a.Name))
        { }

        public AllowedState(string name, IList<string> allowedActions)
            : this(name, allowedActions.ToDictionary(a => a, a => default(IAllowedAction)))
        { }

        public AllowedState(string name, IDictionary<string, IAllowedAction> allowedActions)
        {
            Name = name;
            AllowedActions = new ReadOnlyDictionary<string, IAllowedAction>(allowedActions);
        }

        public AllowedState(params IAllowedAction[] allowedActions)
            : this(new List<IAllowedAction>(allowedActions ?? Enumerable.Empty<IAllowedAction>()))
        { }

        public AllowedState(IList<IAllowedAction> allowedActions)
            : this(allowedActions.ToDictionary(a => a.Name))
        { }

        public AllowedState(IList<string> allowedActions)
            : this(allowedActions.ToDictionary(a => a, a => default(IAllowedAction)))
        { }

        public AllowedState(IDictionary<string, IAllowedAction> allowedActions)
        {
            Name = this.GetType().Name;
            AllowedActions = new ReadOnlyDictionary<string, IAllowedAction>(allowedActions);
        }

        public string Name { get; }
        public IDictionary<string, IAllowedAction> AllowedActions { get; }
    }
}
