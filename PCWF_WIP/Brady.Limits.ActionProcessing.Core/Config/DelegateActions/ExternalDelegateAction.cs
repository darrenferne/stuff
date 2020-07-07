using System;

namespace Brady.Limits.ActionProcessing.Core
{

    public class ExternalDelegateAction<TPayload> : AllowedAction<TPayload>, IExternalAction
        where TPayload : class, IActionRequestPayload
    {
        Func<IActionRequest<TPayload>, IActionProcessingStateChange> _onInvoke;
        Func<IActionRequest<TPayload>, bool> _canInvoke;

        public ExternalDelegateAction(string name, Func<IActionRequest<TPayload>, IActionProcessingStateChange> onInvoke, Func<IActionRequest<TPayload>, bool> canInvoke = null)
            : base(name)
        {
            if (onInvoke is null)
                throw new ArgumentNullException(nameof(onInvoke));
            
            _onInvoke = onInvoke;
            _canInvoke = canInvoke;
        }

        public override bool CanInvoke(IActionRequest<TPayload> request)
        {
            if (_canInvoke is null)
                return base.CanInvoke(request);
            else
                return _canInvoke(request);
        }

        public override IActionResult OnInvoke(IActionRequest<TPayload> request)
        {
            return _onInvoke.Invoke(request);
        }
    }
}
