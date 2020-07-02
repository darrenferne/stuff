﻿using Brady.Limits.ActionProcessing.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class IsAvailable : IsCheckState
    {
        public IsAvailable()
            : base(nameof(IsAvailable),
                  nameof(SubmitContract),
                  nameof(TakeContractOffHold))
        { }
    }
}
