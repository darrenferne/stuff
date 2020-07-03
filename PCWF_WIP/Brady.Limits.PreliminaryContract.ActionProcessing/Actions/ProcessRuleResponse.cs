﻿using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class ProcessRuleResponse : AllowedAction<ActionRequest<RuleResponseProcessingPayload>>, IExternalAction
    {
        public ProcessRuleResponse()
            : base()
        { }

        public override IActionResult OnInvoke(ActionRequest<RuleResponseProcessingPayload> request)
        {
            throw new NotImplementedException();
        }
    }
}
