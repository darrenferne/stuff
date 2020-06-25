using Brady.Limits.ActionProcessing.Core;
using System;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class TakeOffHold : AllowedAction<MakeAvailableRequest>
    {
        public TakeOffHold()
            : base(nameof(TakeOffHold))
        { }

        public override IActionProcessingStateChange OnInvoke(MakeAvailableRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
