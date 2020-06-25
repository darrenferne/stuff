using Brady.Limits.ActionProcessing.Core;
using System;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class Promote : AllowedAction<PromotionRequest>
    {
        public Promote()
            : base(nameof(Promote))
        { }

        public override IActionProcessingStateChange OnInvoke(PromotionRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
