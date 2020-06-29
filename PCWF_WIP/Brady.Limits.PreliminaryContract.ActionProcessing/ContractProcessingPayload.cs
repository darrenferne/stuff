using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class ContractProcessingPayload : IActionRequestPayload
    {
        public ContractProcessingPayload(Contract contract, Guid? trackingReference = null)
        {
            Contract = contract;
            TrackingReference = trackingReference ?? Guid.NewGuid();
        }

        public Contract Contract { get; }

        public Type ObjectType { get; } = typeof(Contract);

        public object Object { get => Contract; }

        public Guid TrackingReference { get; }
    }
}
