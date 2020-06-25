using Brady.Limits.ActionProcessing.Core;
using System.Collections.Generic;
using System.Linq;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class CheckState : AllowedState
    {
        public CheckState(string name, params string[] allowedActions)
            : base(name, (new List<string> {
                           nameof(CheckIsNew),
                           nameof(CheckIsValid),
                           nameof(CheckIsOnHold),
                           nameof(CheckIsInFlight),
                           nameof(CheckIsMaterialChange) })
                        .Union(allowedActions).ToList())
        { }
    }
}
