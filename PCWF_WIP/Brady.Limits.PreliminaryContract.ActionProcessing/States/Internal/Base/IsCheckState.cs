using Brady.Limits.ActionProcessing.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal abstract class IsCheckState : InternalState
    {
        public IsCheckState(string name, params string[] allowedActions)
            : base(name, (new List<string> {
                            nameof(CheckIsNew),
                            nameof(CheckIsValid),
                            nameof(CheckIsAvailable),
                            nameof(CheckIsPendingApproval),
                            //nameof(CheckIsInflight),
                            nameof(CheckIsMaterialChange),
                            nameof(ValidateContract),
                            nameof(NoAction)})
                        .Union(allowedActions).ToList())
        { }
    }
}
