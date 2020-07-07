using System;

namespace Brady.Limits.ActionProcessing.Core.Tests
{
    public class IntegerPayload : ActionProcessingPayload<int>
    {
        public IntegerPayload(int payload, Guid? trackingReference = null) 
            : base(payload, trackingReference)
        { }

        public int Value => (int)Object;
        public static IntegerPayload New(int value, Guid? trackingReference = null) => new IntegerPayload(value, trackingReference);
        public static IntegerPayload Add(IntegerPayload current, int increment) => new IntegerPayload(current.Value + increment, current.TrackingReference);
    }
}
