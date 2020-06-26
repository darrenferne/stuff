using Brady.Limits.ActionProcessing.Core;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class ContractProcessingState : ActionProcessingState
    {
        public ContractProcessingState(string currentState, object externalState = null)
            : base(currentState, externalState)
        { }
        public object ContractState => ExtendedState;
        public bool? IsNew { get; internal set; }
        public bool? IsValid { get; internal set; }
        public bool? IsInFlight { get; internal set; }
        public bool? IsOnHold { get; internal set; }
        public bool? IsMaterialChange { get; internal set; }
        public static ContractProcessingState New(string currentState, object externalState = null) => new ContractProcessingState(currentState, externalState);
    }
}
