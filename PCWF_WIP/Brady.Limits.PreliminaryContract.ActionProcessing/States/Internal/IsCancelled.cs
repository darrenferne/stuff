using Brady.Limits.ActionProcessing.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class IsCancelled : InternalState
    {
        public IsCancelled()
            : base(nameof(IsCancelled),
                  nameof(SubmitContract),
                  nameof(NoAction))
        { }
    }
}
