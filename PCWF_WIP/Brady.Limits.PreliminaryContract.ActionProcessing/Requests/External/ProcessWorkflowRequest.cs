using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class ProcessWorkflowRequest : GatedActionRequest<WorkflowProcessingPayload>
    {
        public ProcessWorkflowRequest(WorkflowProcessingPayload payload)
            : base(nameof(ValidateContract), payload)
        { }

        public static ProcessWorkflowRequest New(WorkflowProcessingPayload payload) => new ProcessWorkflowRequest(payload);
    }
}
