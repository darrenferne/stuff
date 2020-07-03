using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class ProcessRuleResponseRequest : GatedActionRequest<RuleResponseProcessingPayload>
    {
        public ProcessRuleResponseRequest(RuleResponseProcessingPayload payload)
            : base(nameof(ProcessRuleResponse), payload,
                  new GateDescriptor(nameof(IsApproved), new GatedRequestDescriptor(nameof(CheckIsPendingResubmit),
                        new GateDescriptor(nameof(IsPendingResubmit), new ActionRequestDescriptor(typeof(SubmitContract))),
                        new GateDescriptor(nameof(IsNotPendingResubmit), new ActionRequestDescriptor(typeof(NoAction))))),
                  new GateDescriptor(nameof(IsPendingApproval), new ActionRequestDescriptor(typeof(NoAction))))
        { }

        public static ProcessRuleResponseRequest New(RuleResponseProcessingPayload payload) => new ProcessRuleResponseRequest(payload);
    }
}
