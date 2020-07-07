using System;
using System.Collections.Generic;

namespace Brady.Limits.PreliminaryContract.Domain.Models
{
    public class ContractTrackingReference
    {
        public ContractTrackingReference()
            : this(Guid.NewGuid())
        { }
        public ContractTrackingReference(Guid trackingReference)
        {
            TrackingReference = trackingReference;
        }
        
        public Guid TrackingReference { get; }
        public ContractTrackingState State { get; set; }
        public List<ActionTrackingReference> Actions { get; set; }
    }
}
