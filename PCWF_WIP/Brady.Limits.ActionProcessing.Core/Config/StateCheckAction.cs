
using System;

namespace Brady.Limits.ActionProcessing.Core
{
    public abstract class StateCheckAction<TRequest, TResult> : AllowedAction<TRequest, TResult>, IStateCheckAction
        where TRequest : class, IActionRequest
        where TResult : class, IActionProcessingStateCheck
    {
        public StateCheckAction()
            : base()
        { }
        public StateCheckAction(string name)
            : base(name)
        { }

        public IActionProcessingStateCheck InvokeCheckState(IActionRequest request)
        {
            var typedRequest = request as TRequest;
            if (typedRequest is null)
                throw new ArgumentException($"The request is not compatible with the requred request type: '{typeof(TRequest).Name}'");

            return OnInvoke(request as TRequest);
        }
    }    
}
