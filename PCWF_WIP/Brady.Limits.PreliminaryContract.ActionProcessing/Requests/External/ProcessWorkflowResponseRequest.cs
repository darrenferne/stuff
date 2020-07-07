using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;
using System;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class ProcessWorkflowResponseRequest : GatedActionRequest<WorkflowResponseProcessingPayload>
    {
        public ProcessWorkflowResponseRequest(WorkflowResponseProcessingPayload payload)
             : base(nameof(ProcessWorkflowResponse), payload,
                  new GateDescriptor(nameof(IsNotPendingApproval), new GatedRequestDescriptor(nameof(CheckIsPendingResubmit),
                        new GateDescriptor(nameof(IsPendingResubmit), new ActionRequestDescriptor(typeof(SubmitContractRequest))),
                        new GateDescriptor(nameof(IsNotPendingResubmit), new ActionRequestDescriptor(typeof(NoActionRequest))))),
                  new GateDescriptor(nameof(IsPendingApproval), new ActionRequestDescriptor(typeof(NoActionRequest))))
        { }

        public static ProcessWorkflowResponseRequest New(WorkflowResponseProcessingPayload payload) => new ProcessWorkflowResponseRequest(payload);
        public static ProcessWorkflowResponseRequest New(Contract contract, WorkflowResponse response, Guid trackingReference) => new ProcessWorkflowResponseRequest(new WorkflowResponseProcessingPayload(contract, response, trackingReference));
    }
}
