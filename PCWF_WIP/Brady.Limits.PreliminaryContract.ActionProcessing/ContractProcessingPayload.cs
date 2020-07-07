using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;
using System;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class ContractProcessingPayload : IContractProcessingPayload
    {
        public ContractProcessingPayload(Contract contract, Guid? trackingReference = null)
            : this(contract, null, trackingReference ?? Guid.NewGuid())
        { }

        public ContractProcessingPayload(Contract contract, Contract previousVersion, Guid trackingReference)
        {
            Contract = contract;
            PreviousVersion = previousVersion;
            TrackingReference = trackingReference;
        }

        public Contract Contract { get; }

        public Contract PreviousVersion { get; }

        public Type ObjectType { get; } = typeof(Contract);

        public object Object { get => Contract; }

        public Guid TrackingReference { get; }
    }
}
