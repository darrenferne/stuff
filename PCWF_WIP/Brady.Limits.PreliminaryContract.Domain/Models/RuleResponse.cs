using Brady.Limits.PreliminaryContract.Domain.Enums;
using System.Collections.Generic;

namespace Brady.Limits.PreliminaryContract.Domain.Models
{
    public class RuleResponse
    {
        public RuleResponse()
        {
            
        }
        
        public List<TriggeredAction> TriggeredActions { get; set; }
    }
}
