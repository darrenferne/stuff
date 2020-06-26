using Brady.Limits.ActionProcessing.Core;
using System;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class ContractProcessingState : ActionProcessingState
    {
        public ContractProcessingState(string currentState, object externalState = null)
            : base(currentState, externalState)
        { }
        public ContractState ContractState => ExtendedState as ContractState;
        
        public static ContractProcessingState New(string currentState, object externalState = null) => new ContractProcessingState(currentState, externalState);
    }

    public class ContractState: IEquatable<ContractState>
    { 
        public bool? IsNew { get; internal set; }
        public bool? IsValid { get; internal set; }
        public bool? IsInflight { get; internal set; }
        public bool? IsOnHold { get; internal set; }
        public bool? IsMaterialChange { get; internal set; }

        public bool Equals(ContractState other)
        {
            return IsNew.GetValueOrDefault() == other.IsNew.GetValueOrDefault() &&
                IsValid.GetValueOrDefault() == other.IsValid.GetValueOrDefault() &&
                IsInflight.GetValueOrDefault() == other.IsInflight.GetValueOrDefault() &&
                IsOnHold.GetValueOrDefault() == other.IsOnHold.GetValueOrDefault() &&
                IsMaterialChange.GetValueOrDefault() == other.IsMaterialChange.GetValueOrDefault();
        }
    }
}
