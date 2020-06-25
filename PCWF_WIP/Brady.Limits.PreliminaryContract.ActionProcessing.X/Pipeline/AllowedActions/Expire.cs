using Brady.Limits.ActionProcessing.Core;
using System;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class Expire : AllowedAction<ExpirationRequest>
    {
        public Expire()
            : base(nameof(Expire))
        { }

        public override IActionProcessingStateChange OnInvoke(ExpirationRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
