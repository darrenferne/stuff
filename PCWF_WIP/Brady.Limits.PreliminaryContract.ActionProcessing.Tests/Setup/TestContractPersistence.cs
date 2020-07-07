using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;
using System;
using System.Collections.Generic;

namespace Brady.Limits.PreliminaryContract.ActionProcessing.Tests
{
    class TestContractPersistence : IPreliminaryContractPersistence
    {
        private readonly Dictionary<Guid, ContractTrackingReference> _contractStore = new Dictionary<Guid, ContractTrackingReference>();

        public (bool IsSaved, string[] Errors) CreateContractTrackingReference(ContractTrackingReference contractTracking)
        {
            var contractReference = contractTracking.TrackingReference;
            if (!_contractStore.ContainsKey(contractReference))
                _contractStore.Add(contractReference, contractTracking);

            return (true, new string[0]);
        }

        public ContractTrackingReference GetContractTrackingReference(Guid contractReference)
        {
            if (_contractStore.ContainsKey(contractReference))
                return _contractStore[contractReference];
            else
                return null;
        }

        public (bool IsSaved, string[] Errors) UpdateContractTrackingReference(ContractTrackingReference contractTracking)
        {
            var contractReference = contractTracking.TrackingReference;
            if (_contractStore.ContainsKey(contractReference))
                _contractStore[contractReference] = contractTracking;
            else
                _contractStore.Add(contractReference, contractTracking);

            return (true, new string[0]);
        }
    }
}
