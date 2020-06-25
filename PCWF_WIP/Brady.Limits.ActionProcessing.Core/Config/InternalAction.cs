using System;

namespace Brady.Limits.ActionProcessing.Core
{
    public abstract class InternalAction<TRequest> : AllowedAction<TRequest, ActionResponse>, IInternalAction
        where TRequest : class, IActionRequest
    {
        public InternalAction()
            : base()
        { }
        public InternalAction(string name)
            : base(name)
        { }
    }
    
}
