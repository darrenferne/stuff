using System;

namespace Brady.Limits.ActionProcessing.Core
{

    public class ExternalDelegateAction<TPayload> : AllowedAction<ActionRequest<TPayload>>, IExternalAction
        where TPayload : IActionRequestPayload
    {
        Func<ActionRequest<TPayload>, IActionProcessingStateChange> _onInvoke;
        Func<ActionRequest<TPayload>, bool> _canInvoke;

        public ExternalDelegateAction(string name, Func<ActionRequest<TPayload>, IActionProcessingStateChange> onInvoke, Func<ActionRequest<TPayload>, bool> canInvoke = null)
            : base(name)
        {
            if (onInvoke is null)
                throw new ArgumentNullException(nameof(onInvoke));
            
            _onInvoke = onInvoke;
            _canInvoke = canInvoke;
        }

        public override bool CanInvoke(ActionRequest<TPayload> request)
        {
            if (_canInvoke is null)
                return base.CanInvoke(request);
            else
                return _canInvoke(request);
        }

        public override IActionResult OnInvoke(ActionRequest<TPayload> request)
        {
            return _onInvoke.Invoke(request);
        }
    }
}
