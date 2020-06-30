using Brady.Limits.ActionProcessing.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class IsDraft : InternalState
    {
        public IsDraft()
            : base(nameof(IsDraft),
                  nameof(CheckIsValid),
                  nameof(ValidateContract))
        { }
    }
}
