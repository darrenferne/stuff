namespace Brady.Limits.ActionProcessing.Core.Tests
{
    public class TestActionIoC 
    {
        public object SavePayload(object payload)
        {
            LastPayload = payload;
            return payload;
        }

        public object LastPayload { get; set; }
    }
}
