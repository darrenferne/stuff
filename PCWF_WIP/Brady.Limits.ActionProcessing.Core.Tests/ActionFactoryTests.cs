using Akka.TestKit.VsTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using System;

namespace Brady.Limits.ActionProcessing.Core.Tests
{

    [TestClass]
    public partial class ActionFactoryTests : TestKit
    {
        class TestRequirements : IActionProcessorRequirements
        {
            public TestRequirements(IKernel kernel)
            {
                RequestPersistence = new TestRequestPersistence();
                StatePersistence = new TestStatePersistence((r) => "Zero");
                var actionFactory = new TestActionFactory(kernel);

                PipelineConfiguration = new ActionPipelineConfiguration(
                    actionFactory, "TestPipeline",
                    new AllowedState("Zero",
                        nameof(Add)),
                    new AllowedState("Positive",
                        nameof(Add)),
                    new AllowedState("Negative",
                        nameof(Add))
                );
            }

            public IActionProcessorAuthorisation Authorisation { get; }
            public IActionPipelineConfiguration PipelineConfiguration { get; }
            public IActionProcessingRequestPersistence RequestPersistence { get; }
            public IActionProcessingStatePersistence StatePersistence { get; }
            public IActionResponseObserver ActionResponseObserver { get; }
        }
       
        [TestMethod]
        public void Creating_an_action_using_the_action_factory_supplying_a_short_type_name_should_correctly_resolve_and_inject_dependencies()
        {
            var actionFactory = new TestActionFactory();

            actionFactory.Kernel.Bind<TestActionIoC>().ToSelf();

            var action = actionFactory.CreateAction(nameof(Add)) as Add;

            Assert.IsNotNull(action);
            Assert.IsNotNull(action.Injected);
        }

        [TestMethod]
        public void Creating_an_action_using_the_action_factory_supplying_a_fully_qualified_type_name_should_correctly_resolve_and_inject_dependencies()
        {
            var actionFactory = new TestActionFactory();

            actionFactory.Kernel.Bind<TestActionIoC>().ToSelf();

            var action = actionFactory.CreateAction(typeof(Add).FullName) as Add;

            Assert.IsNotNull(action);
            Assert.IsNotNull(action.Injected);
        }

        [TestMethod]
        public void Requesting_an_action_that_requires_creation_via_the_action_factory_should_succeed()
        {
            var kernel = new StandardKernel();
            var statePersistence = new TestActionIoC();

            kernel.Bind<TestActionIoC>().ToConstant(statePersistence);

            var requirements = new TestRequirements(kernel);
            var processor = new ActionProcessor(requirements, Sys);

            processor.Start(false);
            var response = processor.ProcessAction(new ActionRequest<IntegerPayload>(nameof(Add), IntegerPayload.New(0))).Result;

            AwaitAssert(() => {
                Assert.AreEqual(1, (statePersistence?.LastPayload as IntegerPayload)?.Value);
            }, 
            TimeSpan.FromSeconds(1));
        }
    }
}
