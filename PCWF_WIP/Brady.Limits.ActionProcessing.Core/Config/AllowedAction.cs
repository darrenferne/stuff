using System;

namespace Brady.Limits.ActionProcessing.Core
{
    public abstract class AllowedAction<TRequest> : AllowedAction<TRequest, ActionResponse>
        where TRequest : class, IActionRequest
    {
        public AllowedAction()
            : base()
        { }
        public AllowedAction(string name)
            : base(name)
        { }
    }

    public abstract class AllowedAction<TRequest, TResponse> : IAllowedAction
        where TRequest : class, IActionRequest
        where TResponse : class, IActionResponse
    {
        public AllowedAction()
        {
            Name = this.GetType().Name;
        }
        public AllowedAction(string name)
        {
            Name = name;
        }

        protected string FormatError(string error) => $"{Name} Failed: {error}";

        public string Name { get; }

        public virtual bool CanInvoke(TRequest request) => true;

        public abstract IActionProcessingStateChange OnInvoke(TRequest request);

        public virtual IActionProcessingStateChange OnError(TRequest request, Exception ex)
        {
            return new FailureStateChange(request.Payload, request.Context.CurrentState, FormatError(ex.Message));
        }

        IActionProcessingStateChange IAllowedAction.Invoke(IActionRequest request)
        {
            try
            {
                return OnInvoke(request as TRequest);
            }
            catch (Exception ex)
            {
                return OnError(request as TRequest, ex);
            }
        }
    }
}
