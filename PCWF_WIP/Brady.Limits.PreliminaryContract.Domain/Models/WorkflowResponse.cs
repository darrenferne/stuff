using Brady.Limits.PreliminaryContract.Domain.Enums;
using System;
using System.Collections.Generic;

namespace Brady.Limits.PreliminaryContract.Domain.Models
{
    public class WorkflowResponse
    {
        public Guid ActionId { get; set; }
        public string ActionState { get; set; }
        public List<string> Comments { get; set; }
    }
}
