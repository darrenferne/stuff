using System.Collections.Generic;

namespace Brady.Limits.ActionProcessing.Core
{
    public class InternalState : AllowedState, IInternalState
    {
        public InternalState(string name) : base(name)
        {
        }

        public InternalState(params IAllowedAction[] allowedActions) : base(allowedActions)
        {
        }

        public InternalState(IList<IAllowedAction> allowedActions) : base(allowedActions)
        {
        }

        public InternalState(IList<string> allowedActions) : base(allowedActions)
        {
        }

        public InternalState(IDictionary<string, IAllowedAction> allowedActions) : base(allowedActions)
        {
        }

        public InternalState(string name, params IAllowedAction[] allowedActions) : base(name, allowedActions)
        {
        }

        public InternalState(string name, params string[] allowedActions) : base(name, allowedActions)
        {
        }

        public InternalState(string name, IList<IAllowedAction> allowedActions) : base(name, allowedActions)
        {
        }

        public InternalState(string name, IList<string> allowedActions) : base(name, allowedActions)
        {
        }

        public InternalState(string name, IDictionary<string, IAllowedAction> allowedActions) : base(name, allowedActions)
        {
        }
    }
}
