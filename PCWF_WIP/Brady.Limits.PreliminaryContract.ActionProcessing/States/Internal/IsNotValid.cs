using Brady.Limits.ActionProcessing.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class IsNotValid : InternalState
    {
        public IsNotValid()
            : base(nameof(IsNotValid),
                  nameof(CheckIsValid),
                  nameof(ValidateContract),
                  nameof(FailureNotification),
                  nameof(NoAction))
        { }
    }
}
