using Brady.Limits.PreliminaryContract.DataService.Core;
using Brady.Limits.PreliminaryContract.Domain.Enums;
using Brady.Limits.PreliminaryContract.Domain.Models;
using Brady.Limits.PreliminaryContract.Level4File.Concrete;
using Brady.Limits.PreliminaryContract.Tests.Core;
using Brady.Limits.Rules.Domain;
using Brady.Limits.Rules.Processor;
using BWF.DataServices.Core.Concrete.ChangeSets;
using BWF.DataServices.Domain.Models;
using BWF.DataServices.Nancy.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Brady.Limits.PreliminaryContract.DataService.Tests
{
    [TestClass]
    public class RuleResponseProcessorTests : PreliminaryContractDataServiceTestContainer
    {
        private PreliminaryContractRuleResponseProcessor GetRuleResponseProcessor()
        {
            var dshsMock = new Mock<IDataServiceHostSettings>();
            var settingsMock = new Mock<IPreliminaryContractSettings>();
            //var persistence = new Level4ImportPersistence(dshsMock.Object, authorisation, PreliminaryContractDataService, _logger.Object);

            var processor = new PreliminaryContractRuleResponseProcessor(dshsMock.Object, settingsMock.Object, base.PreliminaryContractDataService, base.PreliminaryContractRepository, base.authorisation, _logger.Object);

            return processor;
        }

        private ContractTrackingReference CreateContractTrackingReference(long contractId, Guid itemReference, 
            ContractTrackingState state = ContractTrackingState.ActionsPending, 
            ContractStatus contractStatus = ContractStatus.AvailableForApproval)
        {
            var contract = FluentContract
                .Create()
                .ApplyDefaults()
                .ContractStatus(contractStatus)
                .Save(this); 

            var trackingReference = new ContractTrackingReference() { ContractId = contract.Id, ItemReference = itemReference.ToString(), State = state };

            var roleIds = Enumerable.Empty<long>();
            var changeSet = base.contractTrackingReferenceRecordType.GetNewChangeSet() as ChangeSet<long, ContractTrackingReference>;
            var changeSetSettings = new ProcessChangeSetSettings(username: MrAdmin.Name);
            var itemRef = 1L;

            changeSet.AddCreate(itemRef, trackingReference, roleIds, roleIds);

            var processResult = contractTrackingReferenceRecordType.ProcessChangeSet(PreliminaryContractDataService, changeSet, changeSetSettings);

            return processResult.SuccessfullyCreatedItems[processResult.SuccessfullyCreated[itemRef]] as ContractTrackingReference;
        }

        private List<ActionTrackingReference> CreateActionTrackingReferences(ContractTrackingReference contract, 
            ActionTrackingState state = ActionTrackingState.Pending)
        {   
            var roleIds = Enumerable.Empty<long>();
            var changeSet = base.actionTrackingReferenceRecordType.GetNewChangeSet() as ChangeSet<long, ActionTrackingReference>;
            var changeSetSettings = new ProcessChangeSetSettings(username: MrAdmin.Name);

            for (var itemRef = 1L; itemRef <= 3; itemRef++)
            {
                var action = new ActionTrackingReference() { ItemReference = contract.ItemReference, ActionKey = ActionKeyHelpers.BuildKey(contract.ItemReference, itemRef, itemRef * 10), ActionReference = Guid.NewGuid().ToString(), State = state};
                changeSet.AddCreate(itemRef, action, roleIds, roleIds);
            }

            var processResult = actionTrackingReferenceRecordType.ProcessChangeSet(PreliminaryContractDataService, changeSet, changeSetSettings);

            return processResult
                .SuccessfullyCreatedItems
                .Values
                .Select(i => i as ActionTrackingReference)
                .ToList();
        }

        [TestMethod]
        public void Process_item_reponse_should_return_success_and_populate_action_tracking_references_for_an_item_reponse_with_triggered_actions()
        {
            var itemReference = Guid.NewGuid();
            var contractReference = CreateContractTrackingReference(1, itemReference);
            var processor = GetRuleResponseProcessor();

            var actionKey1 = ActionKeyHelpers.BuildKey(itemReference, 1, 10);
            var actionRef1 = Guid.NewGuid();
            var actionKey2 = ActionKeyHelpers.BuildKey(itemReference, 2, 20);
            var actionRef2 = Guid.NewGuid();
            var actionKey3 = ActionKeyHelpers.BuildKey(itemReference, 3, 30);
            var actionRef3 = Guid.NewGuid();
            var nancyResponse = new ProcessItemNancyResponse()
            {
                Item = new ItemRegistration { ItemReference = itemReference },
                TriggeredActions = new List<TriggeredAction>()
                {
                    new TriggeredAction() { ItemReference = itemReference, ActionKey = actionKey1, ActionReference = actionRef1, ActionState = ActionState.InProgress },
                    new TriggeredAction() { ItemReference = itemReference, ActionKey = actionKey2, ActionReference = actionRef2, ActionState = ActionState.InProgress },
                    new TriggeredAction() { ItemReference = itemReference, ActionKey = actionKey3, ActionReference = actionRef3, ActionState = ActionState.InProgress },
                },
                State = SuccessOrFailureAPIState.Success
            };

            var apiResponse = processor.ProcessItemResponse(nancyResponse).Result;

            Assert.AreEqual(SuccessOrFailureAPIState.Success, apiResponse.State);

            var actionReferences = PreliminaryContractRepository.GetAll<ActionTrackingReference>().ToList();
            Assert.AreEqual(3, actionReferences.Count);

            Assert.IsTrue(actionReferences.Exists(a => a.ActionKey == actionKey1.ToString()));
            Assert.IsTrue(actionReferences.Exists(a => a.ActionReference == actionRef1.ToString()));
            Assert.IsTrue(actionReferences.Exists(a => a.ActionKey == actionKey2.ToString()));
            Assert.IsTrue(actionReferences.Exists(a => a.ActionReference == actionRef2.ToString()));
            Assert.IsTrue(actionReferences.Exists(a => a.ActionKey == actionKey3.ToString()));
            Assert.IsTrue(actionReferences.Exists(a => a.ActionReference == actionRef3.ToString()));

            contractReference = PreliminaryContractRepository.GetSingleOrDefaultWhere<ContractTrackingReference>(c => c.Id == contractReference.Id);
            Assert.AreEqual(ContractTrackingState.ActionsPending, contractReference.State);

            var contract = PreliminaryContractRepository.GetSingleOrDefaultWhere<Contract>(c => c.Id == contractReference.ContractId);
            Assert.AreEqual(ContractStatus.InFlight, contract.ContractStatus);
        }

        [TestMethod]
        public void Process_item_reponse_should_return_success_and_not_populate_action_tracking_references_for_an_item_reponse_with_no_triggered_actions()
        {
            var itemReference = Guid.NewGuid();
            var contractReference = CreateContractTrackingReference(1, itemReference);
            var processor = GetRuleResponseProcessor();

            var nancyResponse = new ProcessItemNancyResponse()
            {
                Item = new ItemRegistration { ItemReference = itemReference },
                TriggeredActions = new List<TriggeredAction>(),
                State = SuccessOrFailureAPIState.Success
            };

            var apiResponse = processor.ProcessItemResponse(nancyResponse).Result;

            Assert.AreEqual(SuccessOrFailureAPIState.Success, apiResponse.State);

            var actionReferences = PreliminaryContractRepository.GetAll<ActionTrackingReference>().ToList();
            Assert.AreEqual(0, actionReferences.Count);

            contractReference = PreliminaryContractRepository.GetSingleOrDefaultWhere<ContractTrackingReference>(c => c.Id == contractReference.Id);
            Assert.AreEqual(ContractTrackingState.NoActions, contractReference.State);

            var contract = PreliminaryContractRepository.GetSingleOrDefaultWhere<Contract>(c => c.Id == contractReference.ContractId);
            Assert.AreEqual(ContractStatus.Approved, contract.ContractStatus);
        }

        [TestMethod]
        public void Process_action_response_should_return_success_and_update_the_action_tracking_reference()
        {
            var itemReference = Guid.NewGuid();
            var contractReference = CreateContractTrackingReference(1, itemReference, contractStatus: ContractStatus.InFlight);
            var trackingReferences = CreateActionTrackingReferences(contractReference);
            var processor = GetRuleResponseProcessor();

            var nancyResponse = new ProcessActionNancyResponse()
            {
                ItemReference = Guid.Parse(trackingReferences[0].ItemReference),
                ActionReference = Guid.Parse(trackingReferences[0].ActionReference),
                ActionKey = trackingReferences[0].ActionKey,
                ActionState = ActionState.InProgress
            };

            var apiResponse = processor.ProcessActionResponse(nancyResponse).Result;

            Assert.AreEqual(SuccessOrFailureAPIState.Success, apiResponse.State);

            var updatedActionReferences = PreliminaryContractRepository.GetAll<ActionTrackingReference>().ToList();
            Assert.AreEqual(3, updatedActionReferences.Count);
            Assert.AreEqual(ActionTrackingState.Processing, updatedActionReferences.Single(a => a.Id == trackingReferences[0].Id).State);
            Assert.AreEqual(trackingReferences[1].State, updatedActionReferences.Single(a => a.Id == trackingReferences[1].Id).State);
            Assert.AreEqual(trackingReferences[2].State, updatedActionReferences.Single(a => a.Id == trackingReferences[2].Id).State);

            contractReference = PreliminaryContractRepository.GetSingleOrDefaultWhere<ContractTrackingReference>(c => c.Id == contractReference.Id);
            Assert.AreEqual(ContractTrackingState.ActionsPending, contractReference.State);

            var contract = PreliminaryContractRepository.GetSingleOrDefaultWhere<Contract>(c => c.Id == contractReference.ContractId);
            Assert.AreEqual(ContractStatus.InFlight, contract.ContractStatus);
        }

        [TestMethod]
        public void Process_action_response_should_return_success_and_update_the_action_tracking_reference_and_contract_tracking_reference_when_all_actions_are_approved()
        {
            var itemReference = Guid.NewGuid();
            var contractReference = CreateContractTrackingReference(1, itemReference);
            var trackingReferences = CreateActionTrackingReferences(contractReference);
            var processor = GetRuleResponseProcessor();

            var nancyResponse = new ProcessActionNancyResponse()
            {
                ItemReference = Guid.Parse(trackingReferences[0].ItemReference),
                ActionKey = trackingReferences[0].ActionKey,
                ActionReference = Guid.Parse(trackingReferences[0].ActionReference),
                ActionState = ActionState.Approved
            };

            var apiResponse = processor.ProcessActionResponse(nancyResponse).Result;

            nancyResponse = new ProcessActionNancyResponse()
            {
                ItemReference = Guid.Parse(trackingReferences[1].ItemReference),
                ActionKey = trackingReferences[1].ActionKey,
                ActionReference = Guid.Parse(trackingReferences[1].ActionReference),
                ActionState = ActionState.Approved
            };

            apiResponse = processor.ProcessActionResponse(nancyResponse).Result;

            nancyResponse = new ProcessActionNancyResponse()
            {
                ItemReference = Guid.Parse(trackingReferences[2].ItemReference),
                ActionKey = trackingReferences[2].ActionKey,
                ActionReference = Guid.Parse(trackingReferences[2].ActionReference),
                ActionState = ActionState.Approved
            };

            apiResponse = processor.ProcessActionResponse(nancyResponse).Result;

            Assert.AreEqual(SuccessOrFailureAPIState.Success, apiResponse.State);

            var updatedActionReferences = PreliminaryContractRepository.GetAll<ActionTrackingReference>().ToList();
            Assert.AreEqual(3, updatedActionReferences.Count);
            Assert.AreEqual(ActionTrackingState.Approved, updatedActionReferences.Single(a => a.Id == trackingReferences[0].Id).State);
            Assert.AreEqual(ActionTrackingState.Approved, updatedActionReferences.Single(a => a.Id == trackingReferences[1].Id).State);
            Assert.AreEqual(ActionTrackingState.Approved, updatedActionReferences.Single(a => a.Id == trackingReferences[2].Id).State);

            contractReference = PreliminaryContractRepository.GetSingleOrDefaultWhere<ContractTrackingReference>(c => c.Id == contractReference.Id);
            Assert.AreEqual(ContractTrackingState.ActionsComplete, contractReference.State);

            var contract = PreliminaryContractRepository.GetSingleOrDefaultWhere<Contract>(c => c.Id == contractReference.ContractId);
            Assert.AreEqual(ContractStatus.Approved, contract.ContractStatus);
        }

        [TestMethod]
        public void Process_action_response_should_return_success_and_update_the_action_tracking_reference_and_contract_tracking_reference_when_any_actions_is_rejected()
        {
            var itemReference = Guid.NewGuid();
            var contractReference = CreateContractTrackingReference(1, itemReference);
            var trackingReferences = CreateActionTrackingReferences(contractReference);
            var processor = GetRuleResponseProcessor();

            var nancyResponse = new ProcessActionNancyResponse()
            {
                ItemReference = Guid.Parse(trackingReferences[0].ItemReference),
                ActionKey = trackingReferences[0].ActionKey,
                ActionReference = Guid.Parse(trackingReferences[0].ActionReference),
                ActionState = ActionState.Approved
            };

            var apiResponse = processor.ProcessActionResponse(nancyResponse).Result;

            nancyResponse = new ProcessActionNancyResponse()
            {
                ItemReference = Guid.Parse(trackingReferences[1].ItemReference),
                ActionKey = trackingReferences[1].ActionKey,
                ActionReference = Guid.Parse(trackingReferences[1].ActionReference),
                ActionState = ActionState.Rejected
            };

            apiResponse = processor.ProcessActionResponse(nancyResponse).Result;

            Assert.AreEqual(SuccessOrFailureAPIState.Success, apiResponse.State);

            var updatedActionReferences = PreliminaryContractRepository.GetAll<ActionTrackingReference>().ToList();
            Assert.AreEqual(3, updatedActionReferences.Count);
            Assert.AreEqual(ActionTrackingState.Approved, updatedActionReferences.Single(a => a.Id == trackingReferences[0].Id).State);
            Assert.AreEqual(ActionTrackingState.Rejected, updatedActionReferences.Single(a => a.Id == trackingReferences[1].Id).State);
            Assert.AreEqual(ActionTrackingState.Pending, updatedActionReferences.Single(a => a.Id == trackingReferences[2].Id).State);

            contractReference = PreliminaryContractRepository.GetSingleOrDefaultWhere<ContractTrackingReference>(c => c.Id == contractReference.Id);
            Assert.AreEqual(ContractTrackingState.ActionsComplete, contractReference.State);

            var contract = PreliminaryContractRepository.GetSingleOrDefaultWhere<Contract>(c => c.Id == contractReference.ContractId);
            Assert.AreEqual(ContractStatus.Rejected, contract.ContractStatus);
        }

        [TestMethod]
        public void Process_action_response_should_return_success_and_update_the_action_tracking_reference_and_contract_tracking_reference_when_contract_tracking_reference_is_actions_complete_and_an_action_is_rejected()
        {
            var itemReference = Guid.NewGuid();
            var contractReference = CreateContractTrackingReference(1, itemReference, ContractTrackingState.ActionsComplete, ContractStatus.Approved);
            var trackingReferences = CreateActionTrackingReferences(contractReference, ActionTrackingState.Approved);
            var processor = GetRuleResponseProcessor();

            var nancyResponse = new ProcessActionNancyResponse()
            {
                ItemReference = Guid.Parse(trackingReferences[0].ItemReference),
                ActionKey = trackingReferences[0].ActionKey,
                ActionReference = Guid.Parse(trackingReferences[0].ActionReference),
                ActionState = ActionState.Rejected
            };

            var apiResponse = processor.ProcessActionResponse(nancyResponse).Result;

            Assert.AreEqual(SuccessOrFailureAPIState.Success, apiResponse.State);

            var updatedActionReferences = PreliminaryContractRepository.GetAll<ActionTrackingReference>().ToList();
            Assert.AreEqual(3, updatedActionReferences.Count);
            Assert.AreEqual(ActionTrackingState.Rejected, updatedActionReferences.Single(a => a.Id == trackingReferences[0].Id).State);
            Assert.AreEqual(ActionTrackingState.Approved, updatedActionReferences.Single(a => a.Id == trackingReferences[1].Id).State);
            Assert.AreEqual(ActionTrackingState.Approved, updatedActionReferences.Single(a => a.Id == trackingReferences[2].Id).State);

            contractReference = PreliminaryContractRepository.GetSingleOrDefaultWhere<ContractTrackingReference>(c => c.Id == contractReference.Id);
            Assert.AreEqual(ContractTrackingState.ActionsComplete, contractReference.State);

            var contract = PreliminaryContractRepository.GetSingleOrDefaultWhere<Contract>(c => c.Id == contractReference.ContractId);
            Assert.AreEqual(ContractStatus.Rejected, contract.ContractStatus);
        }
    }
}
