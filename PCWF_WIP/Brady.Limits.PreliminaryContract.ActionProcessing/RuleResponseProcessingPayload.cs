using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class RuleResponseProcessingPayload : IActionRequestPayload
    {
        public RuleResponseProcessingPayload(RuleResponse ruleResponse, Guid trackingReference)
        {
            RuleResponse = ruleResponse;
            TrackingReference = trackingReference;
        }

        public RuleResponse RuleResponse { get; }

        public Type ObjectType { get; } = typeof(RuleResponse);

        public object Object { get => RuleResponse; }

        public Guid TrackingReference { get; }
    }
}
