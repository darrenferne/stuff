using System;

namespace Brady.Limits.ActionProcessing.Core
{
    public abstract class InternalAction<TRequest, TResult> : AllowedAction<TRequest, TResult>, IInternalAction
        where TRequest : class, IActionRequest
        where TResult : class, IActionResult
    {
        public InternalAction()
            : base()
        { }
        public InternalAction(string name)
            : base(name)
        { }
    }
    
}
