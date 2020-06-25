using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class ProcessContractRequest : ContinuationActionRequest<Contract>
    {
        public ProcessContractRequest(Contract payload)
            : base(nameof(InitialiseProcessState), payload, 
                  new GatedRequestDescriptor(nameof(CheckIsNew), 
                      new GateDescriptor(nameof(IsNew), new ActionRequestDescriptor(typeof(ProcessNewContractRequest))), 
                      new GateDescriptor(nameof(IsNotNew), new ActionRequestDescriptor(typeof(ProcessUpdatedContractRequest)))))
        { }

        public static ProcessContractRequest New(Contract contract) => new ProcessContractRequest(contract);
    }
}
