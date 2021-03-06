﻿using System;
using System.Diagnostics;

namespace Brady.Limits.ActionProcessing.Core
{
    [DebuggerDisplay("Action:{ActionName}, State:{Context.CurrentState?.StateName}, RequestType:{RequestType.Name}, PayloadType:{PayloadType.Name}")]
    public class ActionRequest<TPayload> : Request, IActionRequest<TPayload>, IRequestWithState, IRequestWithContext
        where TPayload : IActionRequestPayload
    {
        protected bool _isRecoveryRequest;

        public ActionRequest(Guid requestId, string actionName, TPayload payload)
            : base(requestId, actionName)
        {
            ActionName = actionName;
            Payload = payload;
        }

        public ActionRequest(string actionName, TPayload payload)
            : this(Guid.NewGuid(), actionName, payload)
        { }
        public string ActionName { get; }
        public IActionRequestContext Context { get; private set; }
        //public IActionProcessingState CurrentState { get; protected set; }
        public TPayload Payload { get; protected set; }
        public Type PayloadType { get; } = typeof(TPayload);

        IActionRequestPayload IActionRequest.Payload => Payload;

        bool IRecoverableRequest.IsRecoveryRequest 
        {
            get => _isRecoveryRequest;
            set => _isRecoveryRequest = value;
        }

        void IRequestWithState.SetState(IActionProcessingState state)
        {
            if (Context is ActionRequestContext)
                Context = ((ActionRequestContext)Context).WithNewState(state);
        }

        void IRequestWithContext.InitialiseContext(IActionRequestContext context)
        {
            if (Context is null)
                Context = context;
            else
                throw new ActionProcessorException("Context is already initialised");
        }
    }
}