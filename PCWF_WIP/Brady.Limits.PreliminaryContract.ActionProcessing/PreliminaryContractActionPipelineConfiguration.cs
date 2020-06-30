using Brady.Limits.ActionProcessing.Core;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class PreliminaryContractActionPipelineConfiguration : ActionPipelineConfiguration
    {
        public PreliminaryContractActionPipelineConfiguration(IKernel kernel)
            : base(new ActionFactory(kernel), "preliminarycontractactionpipeline",
                  new IsDraft(), 
                  new IsNew(),
                  new IsNotNew(),
                  new IsValid(),
                  new IsNotValid(),
                  new IsAvailable(),
                  new IsNotAvailable(),
                  new IsInflight(),
                  new IsNotInflight(),
                  new IsMaterialChange(),
                  new IsNotMaterialChange(),
                  new IsPendingApproval(),
                  new IsRejected(),
                  new IsApproved(),
                  new IsCancelled(),
                  new AvailableForApproval(),
                  new HoldFromApproval())
        { }
    }
}
