using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class ContractProcessingPayload : IActionProcessingPayload
    {
        public ContractProcessingPayload(Contract contract, Guid? trackingReference = null)
        {
            Object = contract;
            TrackingReference = trackingReference ?? Guid.NewGuid();
        }
        public Type ObjectType { get; } = typeof(Contract);

        public object Object { get; }

        public Guid TrackingReference { get; }
    }
}
