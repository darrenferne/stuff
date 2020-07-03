using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class WorkflowResponseProcessingPayload : IActionRequestPayload
    {
        public WorkflowResponseProcessingPayload(WorkflowResponse workflowResponse, Guid trackingReference)
        {
            WorkflowResponse = workflowResponse;
            TrackingReference = trackingReference;
        }

        public WorkflowResponse WorkflowResponse { get; }

        public Type ObjectType { get; } = typeof(WorkflowResponse);

        public object Object { get => WorkflowResponse; }

        public Guid TrackingReference { get; }
    }
}
