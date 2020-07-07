using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;
using System;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class RuleResponseProcessingPayload : ContractProcessingPayload
    {
        public RuleResponseProcessingPayload(Contract contract, RuleResponse ruleResponse, Guid trackingReference)
            : base(contract, trackingReference)
        {
            RuleResponse = ruleResponse;
        }

        public RuleResponse RuleResponse { get; }
    }
}
