using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class CheckIsValid : AllowedAction<ActionRequest<Contract>>
    {
        //IPreliminaryContractRepository _repository;
        //public CheckIsValid(IPreliminaryContractRepository repository)
        //    : base()
        //{
        //    _repository = repository; 
        //}

        //public override IActionProcessingStateChange OnInvoke(ActionRequest<Contract> request)
        //{
        //    var contractProcessingState = request.CurrentState as ContractProcessingState;
        //    if (!contractProcessingState.IsValid.HasValue)
        //    {
        //        var validator = new ContractValidator(_repository, false);
        //        var result = validator.Validate(request.Payload);

        //        contractProcessingState = contractProcessingState.WithIsValid(result.IsValid);
        //    }
        //    var newProcessingState = contractProcessingState.Clone((contractProcessingState.IsValid ?? false) ? nameof(IsValid) : nameof(IsNotValid), contractProcessingState.ContractState);

        //    return new SuccessStateChange(request.Payload, newProcessingState);
        //}

        public CheckIsValid()
        {

        }

        public override IActionProcessingStateChange OnInvoke(ActionRequest<Contract> request)
        {
            var contractProcessingState = request.CurrentState as ContractProcessingState;
            
            var newProcessingState = contractProcessingState.Clone((contractProcessingState.IsNew ?? false) ? nameof(IsNew) : nameof(IsNotNew), contractProcessingState.ContractState);

            return new SuccessStateChange(request.Payload, newProcessingState);
        }
    }
}
