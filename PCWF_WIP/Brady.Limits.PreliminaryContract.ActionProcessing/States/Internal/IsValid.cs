﻿using Brady.Limits.ActionProcessing.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class IsValid : InternalState
    {
        public IsValid()
            : base(nameof(IsValid),
                  nameof(CheckIsValid),
                  nameof(ValidateContract),
                  nameof(CheckIsNew))
        { }
    }
}