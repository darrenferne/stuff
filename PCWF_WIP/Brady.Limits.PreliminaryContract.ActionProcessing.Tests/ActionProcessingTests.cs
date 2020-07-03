using Akka.TestKit.VsTest;
using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Enums;
using Brady.Limits.PreliminaryContract.Domain.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Brady.Limits.PreliminaryContract.ActionProcessing.Tests
{
    [TestClass]
    public class ActionProcessingTests : TestKit
    {

//#if DEBUG
//        TimeSpan assertTimeout = TimeSpan.FromHours(1);
//        TimeSpan assertInterval = TimeSpan.FromMinutes(1);
//#else
        TimeSpan assertTimeout = TimeSpan.FromSeconds(3);
        TimeSpan assertInterval = TimeSpan.FromMilliseconds(100);
//#endif

        IKernel _kernel;
        TestResponseObserver _responseObserver;
        TestRequestPersistence _requestPersistence;
        TestStatePersistence _statePersistence;

        [TestInitialize]
        public void Initialise()
        {
            _kernel = new StandardKernel();

            var mockValdation = new Mock<IPreliminaryContractValidation>();
            mockValdation
                .Setup(pcv => pcv.ValidateContract(It.IsAny<Contract>()))
                .Returns<Contract>((c) => {
                    if (c.ContractValue.GetValueOrDefault() < 0)
                        return (false, new string[] { "'Contract Value' must be greater than zero."});
                    else
                        return (true, new string[] { });
                });

            _kernel.Bind<IPreliminaryContractValidation>().ToMethod(_ => mockValdation.Object);

            _responseObserver = new TestResponseObserver();
            _requestPersistence = new TestRequestPersistence();
            _statePersistence = new TestStatePersistence(r => {

                var contract = r.Payload.Object as Contract;
                var isNew = contract.Id == 0;
                var isPendingApproval = contract.ContractStatus == ContractStatus.InFlight;

                return ("IsDraft", new ContractState(contract.ContractStatus, isNew: isNew, isPendingApproval: isPendingApproval));
            });

            _kernel.Bind<IPreliminaryContractStatePersistence>().ToConstant(_statePersistence);
        }

        private IActionProcessor GetProcessor(bool start = true, bool withRecovery = false)
        {
            var requirements = new TestProcessorRequirements(new PreliminaryContractActionPipelineConfiguration(_kernel), _requestPersistence, _statePersistence, null, _responseObserver);

            var processor = new ActionProcessor(requirements, Sys);

            if (start)
                processor.Start(withRecovery);

            return processor;
        }

        [TestMethod]
        public void Starting_and_stopping_a_new_preliminary_contract_action_processor_should_succeed()
        {
            var processor = GetProcessor(false);

            processor.Start();
            Assert.AreEqual(ActionProcessorState.Started, processor.State);

            processor.Stop();
            Assert.AreEqual(ActionProcessorState.Stopped, processor.State);
        }

        [TestMethod]
        public void Calling_process_action_with_a_process_contract_request_and_a_new_invalid_contract_should_progress_the_trade_to_not_valid()
        {
            var processor = GetProcessor(true);
            var contract = new Contract() { ContractValue = -1 };
            var request = ProcessContractRequest.New(contract);

            var response = processor.ProcessAction(request).Result;

            AwaitAssert(() => {
                Assert.IsTrue(_responseObserver.Responses.Any(r => (r.Request as IActionRequest)?.Context.OriginatingRequest == request && r.GetStateChange().NewState.CurrentState == "IsNotValid"));
            },
            assertTimeout, assertInterval);
        }

        [TestMethod]
        public void Calling_process_action_with_a_process_contract_request_and_a_new_valid_contract_should_progress_the_trade_to_pending_approval()
        {
            var processor = GetProcessor(true);
            var contract = new Contract();
            var request = ProcessContractRequest.New(contract);

            var response = processor.ProcessAction(request).Result;
            
            AwaitAssert(() => {
                Assert.IsTrue(_responseObserver.Responses.Any(r => (r.Request as IActionRequest)?.Context.OriginatingRequest == request && r.GetStateChange().NewState.CurrentState == "IsPendingApproval"));
            },
            assertTimeout, assertInterval);
        }

        [TestMethod]
        public void Calling_process_action_with_a_process_contract_request_and_a_new_on_hold_contract_should_progress_the_trade_to_not_available()
        {
            var processor = GetProcessor(true);
            var contract = new Contract() { GroupHeader = new ContractHeader { HoldFromApproval = true} };
            var request = ProcessContractRequest.New(contract);

            var response = processor.ProcessAction(request).Result;

            AwaitAssert(() => {
                Assert.IsTrue(_responseObserver.Responses.Any(r => (r.Request as IActionRequest)?.Context.OriginatingRequest == request && r.GetStateChange().NewState.CurrentState == "IsNotAvailable"));
            },
            assertTimeout, assertInterval);
        }

        [TestMethod]
        public void Calling_process_action_with_a_process_contract_request_and_an_updated_contract_that_is_not_in_flight_should_progress_the_trade_to_pending_approval()
        {
            var processor = GetProcessor(true);
            var contract = new Contract() { Id = 1 };
            var request = ProcessContractRequest.New(contract);

            var response = processor.ProcessAction(request).Result;

            AwaitAssert(() => {
                Assert.IsTrue(_responseObserver.Responses.Any(r => (r.Request as IActionRequest)?.Context.OriginatingRequest == request && r.GetStateChange().NewState.CurrentState == "IsPendingApproval"));
            },
            assertTimeout, assertInterval);
        }


        [TestMethod]
        public void Calling_process_action_with_a_process_contract_request_and_an_updated_contract_that_is_already_in_flight_should_leave_the_trade_pending_approval()
        {
            var processor = GetProcessor(true);
            var contract = new Contract() { Id = 1, ContractStatus = ContractStatus.InFlight };
            var request = ProcessContractRequest.New(contract);

            var response = processor.ProcessAction(request).Result;

            AwaitAssert(() => {
                Assert.IsTrue(_responseObserver.Responses.Any(r => (r.Request as IActionRequest)?.Context.OriginatingRequest == request && r.GetStateChange().NewState.CurrentState == "IsPendingApproval"));
            },
            assertTimeout, assertInterval);
        }

        [TestMethod]
        public void Calling_process_action_with_a_process_contract_request_and_an_updated_contract_that_is_taken_off_hold_should_progress_the_trade_to_pending_approval()
        {
            var processor = GetProcessor(true);
            var currentContract = new Contract { GroupHeader = new ContractHeader { HoldFromApproval = true } };
            var request = ProcessContractRequest.New(currentContract);
            var trackingRefernence = request.Payload.TrackingReference;

            var response = processor.ProcessAction(request).Result;

            var updatedContract = new Contract { Id = 1, GroupHeader = new ContractHeader { HoldFromApproval = false } };
            request = ProcessContractRequest.New(updatedContract, currentContract, trackingRefernence);

            response = processor.ProcessAction(request).Result;

            AwaitAssert(() => {
                Assert.IsTrue(_responseObserver.Responses.Any(r => (r.Request as IActionRequest)?.Context.OriginatingRequest == request && r.GetStateChange().NewState.CurrentState == "IsPendingApproval"));
            },
            assertTimeout, assertInterval);
        }

        [TestMethod]
        public void Calling_process_action_with_a_process_contract_request_and_an_updated_contract_that_is_in_flight_and_has_been_put_onhold_should_progress_the_trade_to_pending_resubmit()
        {
            var processor = GetProcessor(true);
            var currentContract = new Contract { GroupHeader = new ContractHeader { HoldFromApproval = false } };
            var request = ProcessContractRequest.New(currentContract);
            var trackingRefernence = request.Payload.TrackingReference;

            var response = processor.ProcessAction(request).Result;

            var updatedContract = new Contract { Id = 1, GroupHeader = new ContractHeader { HoldFromApproval = true } };
            request = ProcessContractRequest.New(updatedContract, currentContract, trackingRefernence);

            response = processor.ProcessAction(request).Result;

            AwaitAssert(() => {
                Assert.IsTrue(_responseObserver.Responses.Any(r => (r.Request as IActionRequest)?.Context.OriginatingRequest == request && r.GetStateChange().NewState.CurrentState == "IsPendingResubmit"));
            },
            assertTimeout, assertInterval);
        }
    }
}
