﻿using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class TakeContractOffHold : AllowedAction<TakeContractOffHoldRequest>, IExternalAction
    {
        public TakeContractOffHold()
            : base()
        { }

        public override IActionProcessingStateChange OnInvoke(TakeContractOffHoldRequest request)
        {
            throw new NotImplementedException();
        }
    }
}