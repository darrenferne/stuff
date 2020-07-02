
using System;

namespace Brady.Limits.ActionProcessing.Core
{
    public abstract class StateChangeAction<TRequest, TResult> : AllowedAction<TRequest, TResult>
        where TRequest : class, IActionRequest
        where TResult : class, IActionProcessingStateChange
    {
        public StateChangeAction()
            : base()
        { }
        public StateChangeAction(string name)
            : base(name)
        { }

        public IActionProcessingStateChange InvokeChangeState(IActionRequest request)
        {
            var typedRequest = request as TRequest;
            if (typedRequest is null)
                throw new ArgumentException($"The request is not compatible with the requred requet type: '{typeof(TRequest).Name}'");

            return OnInvoke(request as TRequest);
        }
    }    
}
