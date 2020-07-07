using System;
using System.Collections.Generic;

namespace Brady.Limits.PreliminaryContract.Domain.Models
{
    public class WorkflowResponse
    {
        public Guid ActionReference { get; set; }
        public Guid ActionId { get; set; }
        public ActionState ActionState { get; set; }
        public List<string> Comments { get; set; }
    }
}
