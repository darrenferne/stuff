﻿using System;

namespace Brady.Limits.ActionProcessing.Core
{
    public abstract class AllowedAction<TPayload> : AllowedAction<IActionRequest<TPayload>, IActionResult>
        where TPayload : class, IActionRequestPayload
    {
        public AllowedAction()
            : base()
        { }
        public AllowedAction(string name)
            : base(name)
        { }
    }

    public abstract class AllowedAction<TRequest, TResult> : IAllowedAction
        where TRequest : class, IActionRequest
        where TResult : class, IActionResult
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

        public abstract TResult OnInvoke(TRequest request);

        public virtual IActionResult OnError(TRequest request, Exception ex)
        {
            return new FailureStateChange(request.Payload, request.Context.ProcessingState, FormatError(ex.Message));
        }

        IActionResult IAllowedAction.Invoke(IActionRequest request)
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
