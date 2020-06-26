using Akka.TestKit.VsTest;
using Brady.Limits.ActionProcessing.Core;
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

#if DEBUG
        TimeSpan assertTimeout = TimeSpan.FromHours(1);
        TimeSpan assertInterval = TimeSpan.FromMinutes(1);
#else
        TimeSpan assertTimeout = TimeSpan.FromSeconds(3);
        TimeSpan assertInterval = TimeSpan.FromMilliseconds(100);
#endif

        IKernel _kernel;
        TestResponseObserver _responseObserver;
        TestRequestPersistence _requestPersistence;
        TestStatePersistence _statePersistence;

        [TestInitialize]
        public void Initialise()
        {
            _kernel = new StandardKernel();
            _responseObserver = new TestResponseObserver();
            _requestPersistence = new TestRequestPersistence();
            _statePersistence = new TestStatePersistence();

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
        public void Calling_process_action_with_a_process_contract_request_and_a_new_contract_should_succeed()
        {
            var processor = GetProcessor(true);
            var contract = new Contract();
            var request = ProcessContractRequest.New(contract);

            processor.ProcessAction(request);
            
            AwaitAssert(() => {
                Assert.IsTrue(_responseObserver.Responses.Any(r => r.Request == request && r.StateChange.NewState.StateName == "AvailableForApproval"));
            },
            assertTimeout, assertInterval);
        }
    }
}
