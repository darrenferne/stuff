using System;
using System.Diagnostics;

namespace Brady.Limits.ActionProcessing.Core
{
    [DebuggerDisplay("Action:{ActionName}, State:{CurrentState.Name}")]
    public class ActionRequest<TPayload> : Request, IStatePersistentRequest
        where TPayload : IActionProcessingPayload
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
        public IActionProcessingState CurrentState { get; protected set; }
        public IActionProcessingPayload Payload { get; protected set; }
        public Type PayloadType { get; } = typeof(TPayload);
        
        bool IRecoverableRequest.IsRecoveryRequest 
        {
            get => _isRecoveryRequest;
            set => _isRecoveryRequest = value;
        }

        public void SetState(IActionProcessingState state)
        {
            CurrentState = state;
        }
    }
}