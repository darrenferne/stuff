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
            : base(new ActionFactory(kernel), "preliminarycontractactionpipeline", nameof(Unknown),
                  new Unknown(),
                  new IsNew())
        { }
    }
}
