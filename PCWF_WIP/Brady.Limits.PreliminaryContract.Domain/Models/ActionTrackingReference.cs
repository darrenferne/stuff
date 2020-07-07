using System;

namespace Brady.Limits.PreliminaryContract.Domain.Models
{
    public class ActionTrackingReference
    {
        public ActionTrackingReference()
        { }

        public Guid ActionReference { get; set; }
        public ActionTrackingState State { get; set; }
    }
}
