using Brady.Limits.ActionProcessing.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class IsDraft : IsCheckState
    {
        public IsDraft()
            : base(nameof(IsDraft),
                  nameof(ValidateContract),
                  nameof(NoAction))
        { }
    }
}
