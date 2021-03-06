﻿using Brady.Limits.ActionProcessing.Core;
using Ninject;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class PreliminaryContractActionPipelineConfiguration : ActionPipelineConfiguration
    {
        public PreliminaryContractActionPipelineConfiguration(IKernel kernel)
            : base(new ActionFactory(kernel), "preliminarycontractactionpipeline",
                new IsAvailable(),
                new IsNotAvailable(),
                new IsMaterialChange(),
                new IsNotMaterialChange(),
                new IsNew(),
                new IsNotNew(),
                new IsPendingApproval(),
                new IsNotPendingApproval(),
                new IsPendingCancel(),
                new IsNotPendingCancel(),
                new IsPendingResubmit(),
                new IsNotPendingResubmit(),
                new IsValid(),
                new IsNotValid(),
                new IsDraft(),
                new IsRejected(),
                new IsApproved(),
                new IsCancelled())
        { }
    }
}
