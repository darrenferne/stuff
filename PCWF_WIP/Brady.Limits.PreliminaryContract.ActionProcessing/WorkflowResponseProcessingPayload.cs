using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;
using System;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class WorkflowResponseProcessingPayload : ContractProcessingPayload
    {
        public WorkflowResponseProcessingPayload(Contract contract, WorkflowResponse workflowResponse, Guid trackingReference)
            : base(contract, trackingReference)
        {
            WorkflowResponse = workflowResponse;
        }

        public WorkflowResponse WorkflowResponse { get; }
    }
}
