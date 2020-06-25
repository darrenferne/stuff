
using System;

namespace Brady.Limits.ActionProcessing.Core
{
    public abstract class ExternalAction<TRequest> : AllowedAction<TRequest, ActionResponse>, IExternalAction
        where TRequest : class, IActionRequest
    {
        public ExternalAction()
            : base()
        { }
        public ExternalAction(string name)
            : base(name)
        { }
    }
    
}
