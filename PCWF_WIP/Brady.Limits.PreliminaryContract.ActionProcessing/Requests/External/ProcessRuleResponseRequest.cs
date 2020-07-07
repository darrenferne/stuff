using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;
using System;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class ProcessRuleResponseRequest : GatedActionRequest<RuleResponseProcessingPayload>
    {
        public ProcessRuleResponseRequest(RuleResponseProcessingPayload payload)
            : base(nameof(ProcessRuleResponse), payload,
                  new GateDescriptor(nameof(IsNotPendingApproval), new GatedRequestDescriptor(nameof(CheckIsPendingResubmit),
                        new GateDescriptor(nameof(IsPendingResubmit), new ActionRequestDescriptor(typeof(SubmitContractRequest))),
                        new GateDescriptor(nameof(IsNotPendingResubmit), new ActionRequestDescriptor(typeof(NoActionRequest))))),
                  new GateDescriptor(nameof(IsPendingApproval), new ActionRequestDescriptor(typeof(NoActionRequest))))
        { }

        public static ProcessRuleResponseRequest New(RuleResponseProcessingPayload payload) => new ProcessRuleResponseRequest(payload);
        public static ProcessRuleResponseRequest New(Contract contract, RuleResponse ruleResponse, Guid trackingReference) => new ProcessRuleResponseRequest(new RuleResponseProcessingPayload(contract, ruleResponse, trackingReference));
    }
}
