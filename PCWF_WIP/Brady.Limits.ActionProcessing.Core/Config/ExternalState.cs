using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Brady.Limits.ActionProcessing.Core
{
    public class ExternalState : AllowedState, IExternalState
    {
        public ExternalState(string name) : base(name)
        {
        }

        public ExternalState(params IAllowedAction[] allowedActions) : base(allowedActions)
        {
        }

        public ExternalState(IList<IAllowedAction> allowedActions) : base(allowedActions)
        {
        }

        public ExternalState(IList<string> allowedActions) : base(allowedActions)
        {
        }

        public ExternalState(IDictionary<string, IAllowedAction> allowedActions) : base(allowedActions)
        {
        }

        public ExternalState(string name, params IAllowedAction[] allowedActions) : base(name, allowedActions)
        {
        }

        public ExternalState(string name, params string[] allowedActions) : base(name, allowedActions)
        {
        }

        public ExternalState(string name, IList<IAllowedAction> allowedActions) : base(name, allowedActions)
        {
        }

        public ExternalState(string name, IList<string> allowedActions) : base(name, allowedActions)
        {
        }

        public ExternalState(string name, IDictionary<string, IAllowedAction> allowedActions) : base(name, allowedActions)
        {
        }
    }
}
