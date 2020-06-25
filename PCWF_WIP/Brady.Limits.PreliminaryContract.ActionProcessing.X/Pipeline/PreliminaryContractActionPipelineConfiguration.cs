using Brady.Limits.ActionProcessing.Core;
using Ninject;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class PreliminaryContractActionPipelineConfiguration : ActionPipelineConfiguration
    {
        public PreliminaryContractActionPipelineConfiguration(IKernel kernel)
            : base(new ActionFactory(kernel), "PreliminaryContractActionPipeline", nameof(Saved),
                  new Approved(), 
                  new AvailableForApproval(),
                  new Cancelled(),
                  new Expired(),
                  new HoldFromApproval(),
                  new IsInFlight(),
                  new IsMaterialChange(),
                  new IsNew(),
                  new IsNotInFlight(),
                  new IsNotMaterialChange(),
                  new IsNotNew(),
                  new IsNotOnHold(),
                  new IsNotValid(),
                  new IsOnHold(),
                  new IsValid(),
                  new PendingApproval(),
                  new PendingCancel(),
                  new PendingPromotion(),
                  new Promoted(),
                  new Rejected(), 
                  new Saved())
        { }
    }
}
