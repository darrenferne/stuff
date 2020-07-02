using Brady.Limits.ActionProcessing.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class IsNotAvailable : IsCheckState
    {
        public IsNotAvailable()
            : base(nameof(IsNotAvailable),
                    nameof(PutContractOnHold))
        { }
    }
}
