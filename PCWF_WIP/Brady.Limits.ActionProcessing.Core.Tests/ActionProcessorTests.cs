using Akka.TestKit.VsTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;

namespace Brady.Limits.ActionProcessing.Core.Tests
{
    [TestClass]
    public class ActionProcessorTests : TestKit
    {
        class TestRequirements : IActionProcessorRequirements
        {
            public TestRequirements()
            {
                StatePersistence = new TestStatePersistence();

                RequestPersistence = new TestRequestPersistence(
                    new ActionRequest<IntegerPayload>("LevelUp", IntegerPayload.New(0)),
                    new ActionRequest<IntegerPayload>("LevelUp", IntegerPayload.New(1)),
                    new ActionRequest<IntegerPayload>("LevelUp", IntegerPayload.New(2))
                ); 

                IActionProcessingStateChange LevelUp(ActionRequest<IntegerPayload> r) => LastState = new SuccessStateChange(IntegerPayload.Add(r.Payload as IntegerPayload, 1), TestState.New((r.Payload as IntegerPayload).Value < 3 ? $"Level{(r.Payload as IntegerPayload).Value + 1}" : "Complete"));
                IActionProcessingStateChange LevelDown(ActionRequest<IntegerPayload> r) => LastState = new SuccessStateChange(IntegerPayload.Add(r.Payload as IntegerPayload, -1), TestState.New((r.Payload as IntegerPayload).Value > 1 ? $"Level{(r.Payload as IntegerPayload).Value - 1}" : "Start"));
                IActionProcessingStateChange BrucieBonus(ActionRequest<IntegerPayload> r) => LastState = new SuccessStateChange(IntegerPayload.Add(r.Payload as IntegerPayload, 2), TestState.New((r.Payload as IntegerPayload).Value < 2 ? $"Level{(r.Payload as IntegerPayload).Value + 2}" : "Complete"));
                IActionProcessingStateChange BackToStart(ActionRequest<IntegerPayload> r) => LastState = new SuccessStateChange(IntegerPayload.New(0, r.Payload.TrackingReference), TestState.New("Start"));
                IActionProcessingStateChange SideQuest(ActionRequest<IntegerPayload> r) => LastState = new SuccessStateChange(r.Payload, r.Context.CurrentState);
                IActionProcessingStateChange ToggleUpDown(ActionRequest<IntegerPayload> r) => LastState = (Toggle ? LevelUp(r) : LevelDown(r));

                PipelineConfiguration = new ActionPipelineConfiguration(
                    "TestPipeline", "Start",
                    new AllowedState("Start",
                        new ExternalDelegateAction<IntegerPayload>(nameof(LevelUp), LevelUp),
                        new ExternalDelegateAction<IntegerPayload>(nameof(BrucieBonus), BrucieBonus)),
                    new AllowedState("Level1",
                        new ExternalDelegateAction<IntegerPayload>(nameof(LevelUp), LevelUp),
                        new ExternalDelegateAction<IntegerPayload>(nameof(LevelDown), LevelDown),
                        new ExternalDelegateAction<IntegerPayload>(nameof(BackToStart), BackToStart),
                        new ExternalDelegateAction<IntegerPayload>(nameof(BrucieBonus), BrucieBonus)),
                    new AllowedState("Level2",
                        new ExternalDelegateAction<IntegerPayload>(nameof(LevelUp), LevelUp),
                        new ExternalDelegateAction<IntegerPayload>(nameof(LevelDown), LevelDown),
                        new ExternalDelegateAction<IntegerPayload>(nameof(ToggleUpDown), ToggleUpDown),
                        new ExternalDelegateAction<IntegerPayload>(nameof(BackToStart), BackToStart),
                        new ExternalDelegateAction<IntegerPayload>(nameof(BrucieBonus), BrucieBonus),
                        new ExternalDelegateAction<IntegerPayload>(nameof(SideQuest), SideQuest)),
                    new AllowedState("Level3",
                        new ExternalDelegateAction<IntegerPayload>(nameof(LevelUp), LevelUp),
                        new ExternalDelegateAction<IntegerPayload>(nameof(LevelDown), LevelDown),
                        new ExternalDelegateAction<IntegerPayload>(nameof(BackToStart), BackToStart)),
                    new AllowedState("Complete",
                        new ExternalDelegateAction<IntegerPayload>(nameof(BackToStart), BackToStart))
                );
            }

            public bool Toggle { get; set; }
            public IActionProcessingStateChange LastState { get; internal set; }

            public IActionProcessorAuthorisation Authorisation { get; }
            public IActionPipelineConfiguration PipelineConfiguration { get; }
            internal Mock<IActionProcessingRequestPersistence> PipelinePersistenceMock { get; }
            public IActionProcessingRequestPersistence RequestPersistence { get; }
            public IActionProcessingStatePersistence StatePersistence { get; }
            public IActionResponseObserver ActionResponseObserver { get; }
        }

        TestRequirements _requirements;

        [TestInitialize]
        public void Initialise()
        {
            _requirements = new TestRequirements();
        }

        private IActionProcessor GetProcessor(bool start = true, bool withRecovery = false)
        {
            var processor = new ActionProcessor(_requirements, Sys);

            if (start)
                processor.Start(withRecovery);

            return processor;
        }

        [TestMethod]
        public void Starting_and_stopping_a_new_action_processor_should_succeed()
        {
            var processor = GetProcessor(false);

            processor.Start();
            Assert.AreEqual(ActionProcessorState.Started, processor.State);

            processor.Stop();
            Assert.AreEqual(ActionProcessorState.Stopped, processor.State);
        }

        [TestMethod]
        public void Starting_a_new_action_processor_with_recovery_option_should_succeed()
        {
            var processor = GetProcessor(false);

            processor.Start(true);

            AwaitAssert(() =>
            {
                _requirements.PipelinePersistenceMock.Verify(p => p.GetPendingRequests(), Times.Once);
                _requirements.PipelinePersistenceMock.Verify(p => p.DeletePendingRequest(It.IsAny<Guid>()), Times.Exactly(3));
            },
            TimeSpan.FromSeconds(1));
        }

        //[TestMethod]
        //public void Sending_a_process_action_request_before_the_processor_has_started_should_fail()
        //{
        //    var processor = GetProcessor(false);

        //    try
        //    {
        //        var response = processor.ProcessAction<ActionRequest<IntegerPayload>>(new ActionRequest<IntegerPayload>("LevelUp", IntegerPayload.New(1))).Result;
        //    }
        //    catch (Exception ex)
        //    {
        //        Assert.AreEqual(typeof(ActionProcessorException), ex.GetType());
        //        Assert.AreEqual("ProcessAction requests cannot be submitted before the processor has been started", ex.Message);
        //    }
        //}

        //[TestMethod]
        //public void Sending_an_invalid_process_action_request_for_an_invalid_state_should_fail()
        //{
        //    var processor = GetProcessor();

        //    var response = processor.ProcessAction<ActionRequest<IntegerPayload>>(new ActionRequest<IntegerPayload>("CanIBreakIt", IntegerPayload.New(-1))).Result;

        //    Assert.AreEqual(typeof(UnhandledResponse), response.GetType());
        //    Assert.AreEqual("Request Rejected. The requested action 'CanIBreakIt' is not valid.", ((UnhandledResponse)response).Message);
        //}

        //[TestMethod]
        //public void Sending_an_invalid_process_action_request_for_a_valid_state_should_fail()
        //{
        //    var processor = GetProcessor();

        //    var response = processor.ProcessAction<ActionRequest<IntegerPayload>>(new ActionRequest<IntegerPayload>("CanIBreakIt", IntegerPayload.New(-1))).Result;

        //    Assert.AreEqual(typeof(UnhandledResponse), response.GetType());
        //    Assert.AreEqual("Request Rejected. The requested action 'CanIBreakIt' is not valid.", ((UnhandledResponse)response).Message);
        //}

        //[TestMethod]
        //public void Sending_an_valid_process_action_request_for_a_valid_state_should_succeed()
        //{
        //    var processor = GetProcessor();

        //    var response = processor.ProcessAction<ActionRequest<IntegerPayload>>(new ActionRequest<IntegerPayload>("LevelUp", IntegerPayload.New(0))).Result;

        //    Assert.AreEqual(typeof(ActionResponse), response.GetType());
        //    Assert.AreEqual(1, ((ActionResponse)response).StateChange.NewPayload);
        //    Assert.AreEqual("Level1", ((ActionResponse)response).StateChange.NewState.StateName);

        //    _requirements.PipelinePersistenceMock.Verify(p => p.SavePendingRequest(It.IsAny<IActionRequest>()), Times.Once);
        //    _requirements.PipelinePersistenceMock.Verify(p => p.DeletePendingRequest(It.IsAny<Guid>()), Times.Once);
        //}

        //[TestMethod]
        //public void Progressing_through_valid_states_should_succeed()
        //{
        //    var processor = GetProcessor();
        //    var trackingReference = Guid.NewGuid();

        //    var response = processor.ProcessAction<ActionRequest<IntegerPayload>>(new ActionRequest<IntegerPayload>("LevelUp", IntegerPayload.New(0, trackingReference))).Result;
            
        //    Assert.AreEqual(typeof(ActionResponse), response.GetType());
        //    Assert.AreEqual(1, ((ActionResponse)response).StateChange.NewPayload);
        //    Assert.AreEqual("Level1", ((ActionResponse)response).StateChange.NewState.StateName);

        //    response = processor.ProcessAction<ActionRequest<IntegerPayload>>(new ActionRequest<IntegerPayload>("LevelUp", IntegerPayload.New(1, trackingReference))).Result;

        //    Assert.AreEqual(typeof(ActionResponse), response.GetType());
        //    Assert.AreEqual(2, ((ActionResponse)response).StateChange.NewPayload);
        //    Assert.AreEqual("Level2", ((ActionResponse)response).StateChange.NewState.StateName);

        //    response = processor.ProcessAction<ActionRequest<IntegerPayload>>(new ActionRequest<IntegerPayload>("SideQuest", IntegerPayload.New(1, trackingReference))).Result;

        //    Assert.AreEqual(typeof(UnhandledResponse), response.GetType());

        //    response = processor.ProcessAction<ActionRequest<IntegerPayload>>(new ActionRequest<IntegerPayload>("LevelUp", IntegerPayload.New(2, trackingReference))).Result;

        //    Assert.AreEqual(typeof(ActionResponse), response.GetType());
        //    Assert.AreEqual(3, ((ActionResponse)response).StateChange.NewPayload);
        //    Assert.AreEqual("Level3", ((ActionResponse)response).StateChange.NewState.StateName);

        //    response = processor.ProcessAction<ActionRequest<IntegerPayload>>(new ActionRequest<IntegerPayload>("SideQuest", IntegerPayload.New(2, trackingReference))).Result;

        //    Assert.AreEqual(typeof(ActionResponse), response.GetType());
        //    Assert.AreEqual(((IActionRequest)response.Request).CurrentState.CurrentStateName, ((ActionResponse)response).StateChange.NewState.StateName);

        //    response = processor.ProcessAction<ActionRequest<IntegerPayload>>(new ActionRequest<IntegerPayload>("LevelDown", IntegerPayload.New(3, trackingReference))).Result;

        //    Assert.AreEqual(typeof(ActionResponse), response.GetType());
        //    Assert.AreEqual(2, ((ActionResponse)response).StateChange.NewPayload);
        //    Assert.AreEqual("Level2", ((ActionResponse)response).StateChange.NewState.StateName);

        //    response = processor.ProcessAction<ActionRequest<IntegerPayload>>(new ActionRequest<IntegerPayload>("BrucieBonus", IntegerPayload.New(2, trackingReference))).Result;

        //    Assert.AreEqual(typeof(ActionResponse), response.GetType());
        //    Assert.AreEqual(4, ((ActionResponse)response).StateChange.NewPayload);
        //    Assert.AreEqual("Complete", ((ActionResponse)response).StateChange.NewState.StateName);

        //    response = processor.ProcessAction<ActionRequest<IntegerPayload>>(new ActionRequest<IntegerPayload>("BackToStart", IntegerPayload.New(3, trackingReference))).Result;

        //    Assert.AreEqual(typeof(ActionResponse), response.GetType());
        //    Assert.AreEqual(0, ((ActionResponse)response).StateChange.NewPayload);
        //    Assert.AreEqual("Start", ((ActionResponse)response).StateChange.NewState.StateName);
        //}

        //[TestMethod]
        //public void Processing_a_continuation_request_should_succeed()
        //{
        //    var processor = GetProcessor();
        //    var completionRequest = new ContinuationActionRequest<IntegerPayload>("LevelUp", IntegerPayload.New(0),
        //          new ActionRequestDescriptor("LevelUp"),
        //          new ActionRequestDescriptor("SideQuest"),
        //          new ActionRequestDescriptor("LevelUp"),
        //          new ActionRequestDescriptor("LevelUp"));
        
        //    var response = processor.ProcessAction<ContinuationActionRequest<IntegerPayload>>(completionRequest).Result;

        //    AwaitAssert(() =>
        //    {
        //        Assert.AreEqual("Complete", _requirements.LastState.NewState.StateName);
        //        Assert.AreEqual(4, _requirements.LastState.NewPayload);
        //    },
        //    TimeSpan.FromSeconds(3));
        //}

        //[TestMethod]
        //public void Processing_a_gated_request_should_succeed()
        //{
        //    var processor = GetProcessor();
        //    var trackngReference = Guid.NewGuid();

        //    _requirements.Toggle = true; //Should trigger Up
            
        //    var gatedRequest = new GatedActionRequest<IntegerPayload>("ToggleUpDown", IntegerPayload.New(2, trackngReference),
        //          new GateDescriptor("Level1", new ActionRequestDescriptor("LevelDown")),
        //          new GateDescriptor("Level3", new ActionRequestDescriptor("LevelUp")));

        //    processor.ProcessAction<GatedActionRequest<IntegerPayload>>(gatedRequest);

        //    AwaitAssert(() =>
        //    {
        //        Assert.AreEqual("Complete", _requirements.LastState.NewState.StateName);
        //        Assert.AreEqual(4, _requirements.LastState.NewPayload);
        //    },
        //    TimeSpan.FromSeconds(3));

        //    _requirements.Toggle = false; //Should trigger Down

        //    gatedRequest = new GatedActionRequest<IntegerPayload>("ToggleUpDown", IntegerPayload.New(2, trackngReference),
        //          new GateDescriptor("Level1", new ActionRequestDescriptor("LevelDown")),
        //          new GateDescriptor("Level3", new ActionRequestDescriptor("LevelUp")));

        //    processor.ProcessAction<GatedActionRequest<IntegerPayload>>(gatedRequest);

        //    AwaitAssert(() =>
        //    {
        //        Assert.AreEqual("Start", _requirements.LastState.NewState.StateName);
        //        Assert.AreEqual(0, _requirements.LastState.NewPayload);
        //    },
        //    TimeSpan.FromSeconds(3));
        //}
    }
}
