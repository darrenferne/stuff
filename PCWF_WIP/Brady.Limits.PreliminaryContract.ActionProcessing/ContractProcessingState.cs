using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Enums;
using System;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class FluentValue<T>
    {
        bool _isSet;
        T _value;
        public FluentValue()
            : this(false, default(T))
        { }
        public FluentValue(T newValue)
            : this(true, newValue)
        { }
        public FluentValue(bool isSet, T newValue)
        {
            _isSet = isSet;
            _value = newValue;
        }
        
        public bool IsSet => _isSet;
        public T NewValue
        {
            get => _value;
            set {
                _value = value;
                _isSet = true;
            }
        }
    }

    public class FluentProcessingState
    {
        Func<string> _getCurrentState = null;

        FluentValue<ContractStatus?> _newContractStatus = new FluentValue<ContractStatus?>();
        FluentValue<bool?> _newIsAvailable = new FluentValue<bool?>();
        FluentValue<bool?> _newIsMaterialChange = new FluentValue<bool?>();
        FluentValue<bool?> _newIsNew = new FluentValue<bool?>();
        FluentValue<bool?> _newIsPendingApproval = new FluentValue<bool?>();
        FluentValue<bool?> _newIsPendingCancel = new FluentValue<bool?>();
        FluentValue<bool?> _newIsPendingResubmit = new FluentValue<bool?>();
        FluentValue<bool?> _newIsValid = new FluentValue<bool?>();

        public FluentProcessingState(ContractProcessingState initialState)
        {
            InitialState = initialState;
        }
        internal ContractProcessingState InitialState { get; }

        public FluentProcessingStateConjuction SetCurrentFromIsAvailable()
        {
            _getCurrentState = () =>
            {
                var isAvailable = _newIsAvailable.IsSet ? _newIsAvailable.NewValue : InitialState.ContractState.IsAvailable.GetValueOrDefault();
                return isAvailable.GetValueOrDefault() ? nameof(IsAvailable) : nameof(IsNotAvailable);
            };
            return new FluentProcessingStateConjuction(this);
        }

        public FluentProcessingStateConjuction SetCurrentFromIsMaterialChange()
        {
            _getCurrentState = () =>
            {
                var isMaterialChange = _newIsMaterialChange.IsSet ? _newIsMaterialChange.NewValue : InitialState.ContractState.IsMaterialChange.GetValueOrDefault();
                return isMaterialChange.GetValueOrDefault() ? nameof(IsMaterialChange) : nameof(IsNotMaterialChange);
            };
            return new FluentProcessingStateConjuction(this);
        }
        public FluentProcessingStateConjuction SetCurrentFromIsNew()
        {
            _getCurrentState = () =>
            {
                var isNew = _newIsNew.IsSet ? _newIsNew.NewValue : InitialState.ContractState.IsNew.GetValueOrDefault();
                return isNew.GetValueOrDefault() ? nameof(IsNew) : nameof(IsNotNew);
            };
            return new FluentProcessingStateConjuction(this);
        }
        public FluentProcessingStateConjuction SetCurrentFromIsPendingApproval()
        {
            _getCurrentState = () =>
            {
                var isPendingApproval = _newIsPendingApproval.IsSet ? _newIsPendingApproval.NewValue : InitialState.ContractState.IsPendingApproval.GetValueOrDefault();
                return isPendingApproval.GetValueOrDefault() ? nameof(IsPendingApproval) : nameof(IsNotPendingApproval);
            };
            return new FluentProcessingStateConjuction(this);
        }
        public FluentProcessingStateConjuction SetCurrentFromIsPendingCancel()
        {
            _getCurrentState = () =>
            {
                var isPendingCancel = _newIsPendingCancel.IsSet ? _newIsPendingCancel.NewValue : InitialState.ContractState.IsPendingCancel.GetValueOrDefault();
                return isPendingCancel.GetValueOrDefault() ? nameof(IsPendingCancel) : nameof(IsNotPendingCancel);
            };
            return new FluentProcessingStateConjuction(this);
        }
        public FluentProcessingStateConjuction SetCurrentFromIsPendingResubmit()
        {
            _getCurrentState = () =>
            {
                var isPendingResubmit = _newIsPendingResubmit.IsSet ? _newIsPendingResubmit.NewValue : InitialState.ContractState.IsPendingResubmit.GetValueOrDefault();
                return isPendingResubmit.GetValueOrDefault() ? nameof(IsPendingResubmit) : nameof(IsNotPendingResubmit);
            };
            return new FluentProcessingStateConjuction(this);
        }
        public FluentProcessingStateConjuction SetCurrentFromIsValid()
        {
            _getCurrentState = () =>
            {
                var isValid = _newIsValid.IsSet ? _newIsValid.NewValue : InitialState.ContractState.IsValid.GetValueOrDefault();
                return isValid.GetValueOrDefault() ? nameof(IsValid) : nameof(IsNotValid);
            };
            return new FluentProcessingStateConjuction(this);
        }
        public FluentProcessingStateConjuction SetContractStatus(ContractStatus? contractStatus)
        {
            _newContractStatus.NewValue = contractStatus;
            
            return new FluentProcessingStateConjuction(this);
        }
        public FluentProcessingStateConjuction SetIsAvailable(bool? isAvailable)
        {
            _newIsAvailable.NewValue = isAvailable;
            return new FluentProcessingStateConjuction(this);
        }
        public FluentProcessingStateConjuction SetIsMaterialChange(bool? isMaterialChange)
        {
            _newIsMaterialChange.NewValue = isMaterialChange;
            return new FluentProcessingStateConjuction(this);
        }
        public FluentProcessingStateConjuction SetIsPendingApproval(bool? isPendingApproval)
        {
            _newIsPendingApproval.NewValue = isPendingApproval;
            return new FluentProcessingStateConjuction(this);
        }
        public FluentProcessingStateConjuction SetIsPendingCancel(bool? isPendingCancel)
        {
            _newIsPendingCancel.NewValue = isPendingCancel;
            return new FluentProcessingStateConjuction(this);
        }
        public FluentProcessingStateConjuction SetIsPendingResubmit(bool? isPendingResubmit)
        {
            _newIsPendingResubmit.NewValue = isPendingResubmit;
            return new FluentProcessingStateConjuction(this);
        }
        public FluentProcessingStateConjuction SetIsNew(bool? isNew)
        {
            _newIsNew.NewValue = isNew;
            return new FluentProcessingStateConjuction(this);
        }
        public FluentProcessingStateConjuction SetIsValid(bool? isValid)
        {
            _newIsValid.NewValue = isValid;
            return new FluentProcessingStateConjuction(this);
        }
        internal ContractProcessingState GetFinalState()
        {
            var newContractState = new ContractState(
                _newContractStatus.IsSet ? _newContractStatus.NewValue.Value : InitialState.ContractState.ContractStatus,
                _newIsAvailable.IsSet ? _newIsAvailable.NewValue : InitialState.ContractState.IsAvailable,
                _newIsMaterialChange.IsSet ? _newIsMaterialChange.NewValue : InitialState.ContractState.IsMaterialChange,
                _newIsNew.IsSet ? _newIsNew.NewValue : InitialState.ContractState.IsNew,
                _newIsPendingApproval.IsSet ? _newIsPendingApproval.NewValue : InitialState.ContractState.IsPendingApproval,
                _newIsPendingCancel.IsSet ? _newIsPendingCancel.NewValue : InitialState.ContractState.IsPendingCancel,
                _newIsPendingResubmit.IsSet ? _newIsPendingResubmit.NewValue : InitialState.ContractState.IsPendingResubmit,
                _newIsValid.IsSet ? _newIsValid.NewValue : InitialState.ContractState.IsValid);
            
            return new ContractProcessingState(_getCurrentState?.Invoke() ?? InitialState.CurrentState, newContractState);
        }
    }

    public class FluentProcessingStateConjuction
    {
        public FluentProcessingStateConjuction(FluentProcessingState fluentState)
        {
            FluentState = fluentState;
        }
        internal FluentProcessingState FluentState { get; }

        internal ContractProcessingState GetFinalState()
        {
            return FluentState.GetFinalState();
        }
    }

    public static class FluentProcessingStateExtensions
    {
        public static ContractProcessingState Clone(this ContractProcessingState state, Func<FluentProcessingState, FluentProcessingStateConjuction> builder)
        {
            if (builder is null)
                return state;
            else
            {
                var fluentState = builder.Invoke(new FluentProcessingState(state));
                return fluentState.GetFinalState();
            }
        }

        public static FluentProcessingState And(this FluentProcessingStateConjuction conjunction)
        {
            return conjunction.FluentState;
        }
    }

    public class ContractProcessingState : ActionProcessingState
    {
        public ContractProcessingState(string currentState = null, object externalState = null)
            : base(currentState, externalState)
        { }
        public ContractState ContractState => ExtendedState as ContractState;
        
        public static ContractProcessingState New(string currentState, object externalState = null) => new ContractProcessingState(currentState, externalState);
    }

    public class ContractState: IEquatable<ContractState>
    {
        public ContractState(ContractStatus contractStatus, bool? isAvailable = null, bool? isMaterialChange = null, bool? isNew = null, bool? isPendingApproval = null, bool? isPendingCancel = null, bool? isPendingResubmit = null, bool? isValid = null)
        {
            ContractStatus = contractStatus;
            IsAvailable = isAvailable;
            IsMaterialChange = isMaterialChange;
            IsNew = isNew;
            IsPendingApproval = isPendingApproval;
            IsPendingCancel = isPendingCancel;
            IsPendingResubmit = isPendingResubmit;
            IsValid = isValid;
        }

        public ContractStatus ContractStatus { get; internal set; }
        public bool? IsAvailable { get; internal set; }
        public bool? IsMaterialChange { get; internal set; }
        public bool? IsNew { get; internal set; }
        public bool? IsPendingApproval { get; internal set; }
        public bool? IsPendingCancel { get; internal set; }
        public bool? IsPendingResubmit { get; internal set; }
        public bool? IsValid { get; internal set; }

        public bool Equals(ContractState other)
        {
            return ContractStatus == other.ContractStatus &&
                IsAvailable.GetValueOrDefault() == other.IsAvailable.GetValueOrDefault() &&
                IsMaterialChange.GetValueOrDefault() == other.IsMaterialChange.GetValueOrDefault() &&
                IsNew.GetValueOrDefault() == other.IsNew.GetValueOrDefault() &&
                IsPendingApproval.GetValueOrDefault() == other.IsPendingApproval.GetValueOrDefault() &&
                IsPendingCancel.GetValueOrDefault() == other.IsPendingCancel.GetValueOrDefault() &&
                IsPendingResubmit.GetValueOrDefault() == other.IsPendingResubmit.GetValueOrDefault() &&
                IsValid.GetValueOrDefault() == other.IsValid.GetValueOrDefault();
        }
    }
}
