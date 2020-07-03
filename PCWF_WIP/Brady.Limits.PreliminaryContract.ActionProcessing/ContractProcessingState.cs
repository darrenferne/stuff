using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Enums;
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
        public ContractState(ContractStatus contractStatus, bool? isNew = null, bool? isValid = null, bool? isPendingApproval = null, bool? isAvailable = null, bool? isMaterialChange = null, bool? isPendingResubmit = null)
        {
            ContractStatus = ContractStatus;
            IsNew = isNew;
            IsValid = isValid;
            IsPendingApproval = isPendingApproval;
            IsAvailable = isAvailable;
            IsMaterialChange = isMaterialChange;
            IsPendingResubmit = IsPendingResubmit;
        }
        public ContractStatus ContractStatus { get; set; }
        public bool? IsNew { get; internal set; }
        public bool? IsValid { get; internal set; }
        public bool? IsPendingApproval { get; internal set; }
        public bool? IsPendingResubmit { get; internal set; }
        public bool? IsAvailable { get; internal set; }
        public bool? IsMaterialChange { get; internal set; }

        public bool Equals(ContractState other)
        {
            return ContractStatus == other.ContractStatus &&
                IsNew.GetValueOrDefault() == other.IsNew.GetValueOrDefault() &&
                IsValid.GetValueOrDefault() == other.IsValid.GetValueOrDefault() &&
                IsPendingApproval.GetValueOrDefault() == other.IsPendingApproval.GetValueOrDefault() &&
                IsAvailable.GetValueOrDefault() == other.IsAvailable.GetValueOrDefault() &&
                IsMaterialChange.GetValueOrDefault() == other.IsMaterialChange.GetValueOrDefault() &&
                IsPendingResubmit.GetValueOrDefault() == other.IsPendingResubmit.GetValueOrDefault();
        }
    }
}
