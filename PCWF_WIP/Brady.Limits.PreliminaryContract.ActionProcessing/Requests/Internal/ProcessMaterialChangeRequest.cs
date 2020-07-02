using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class ProcessMaterialChangeRequest : GatedActionRequest<ContractProcessingPayload>
    {
        public ProcessMaterialChangeRequest(ContractProcessingPayload payload)
            : base(nameof(CheckIsPendingApproval), payload,
                  new GateDescriptor(nameof(IsPendingApproval), new ActionRequestDescriptor(typeof(ResubmitContractRequest))),
                  new GateDescriptor(nameof(IsNotPendingApproval), new ActionRequestDescriptor(typeof(ProcessNewContractRequest))))
        { }

        public static ProcessMaterialChangeRequest New(ContractProcessingPayload payload) => new ProcessMaterialChangeRequest(payload);
        public static ProcessMaterialChangeRequest New(Contract contract, Guid? trackingReference = null) => new ProcessMaterialChangeRequest(new ContractProcessingPayload(contract, trackingReference));
    }
}
