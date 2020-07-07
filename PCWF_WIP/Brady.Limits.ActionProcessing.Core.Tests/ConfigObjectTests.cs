using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Brady.Limits.ActionProcessing.Core.Tests
{
    [TestClass]
    public class ConfigObjectTests
    {
        class TestAction : AllowedAction<IntegerPayload>
        {
            public TestAction()
                : base()
            { }

            public TestAction(string name)
                : base(name)
            { }

            public override IActionResult OnInvoke(IActionRequest<IntegerPayload> request)
            {
                return new StateChangeResult(IntegerPayload.Add(request.Payload as IntegerPayload, 1), TestState.New("OneMore"), "Happy Path");
            }
        }

        [TestMethod]
        public void Creating_a_new_allowedaction_object_with_custom_name_should_correctly_set_properties()
        {
            var expectedName = "TestActionName";
            var action = new TestAction(expectedName);

            Assert.AreEqual(expectedName, action.Name);
        }

        [TestMethod]
        public void Creating_a_new_allowedaction_object_should_correctly_set_properties()
        {
            var expectedName = nameof(TestAction);
            var action = new TestAction();

            Assert.AreEqual(expectedName, action.Name);
        }

        [TestMethod]
        public void Invoking_an_allowedaction_should_call_the_correct_override()
        {
            IAllowedAction action = new TestAction();

            var currentPayload = IntegerPayload.New(1);
            var expectedState = TestState.New("OneMore");
            var expectedValue = 2;
            var stateChange = action.Invoke(new ActionRequest<IntegerPayload>("TestAction", currentPayload)).GetStateChange();

            Assert.AreEqual(expectedValue, stateChange.NewPayload.Object);
            Assert.AreEqual(expectedState.CurrentState, stateChange.NewState.CurrentState);
            Assert.AreEqual(1, stateChange.Messages.Count());
            Assert.AreEqual("Happy Path", stateChange.Messages.First());
        }

        [TestMethod]
        public void Creating_a_new_allowedstate_object_with_no_actions_should_correctly_set_properties()
        {
            var expectedName = "Test";
            var state = new AllowedState(expectedName);

            Assert.AreEqual(expectedName, state.Name);
            Assert.IsNotNull(state.AllowedActions);
            Assert.AreEqual(0, state.AllowedActions.Count);
        }

        [TestMethod]
        public void Creating_a_new_allowedstate_object_with_string_actions_should_correctly_set_properties()
        {
            var expectedName = "Test";
            var expectedAction = "TestAction";

            var state = new AllowedState(expectedName, expectedAction);

            Assert.AreEqual(expectedName, state.Name);
            Assert.IsNotNull(state.AllowedActions);
            Assert.AreEqual(1, state.AllowedActions.Count);
            Assert.IsTrue(state.AllowedActions.ContainsKey(expectedAction));
            Assert.IsNull(state.AllowedActions[expectedAction]);
        }

        [TestMethod]
        public void Creating_a_new_allowedstate_object_with_a_list_of_string_actions_should_correctly_set_properties()
        {
            var expectedName = "Test";
            var expectedAction = "TestAction";

            var state = new AllowedState(expectedName, new List<string> { expectedAction });

            Assert.AreEqual(expectedName, state.Name);
            Assert.IsNotNull(state.AllowedActions);
            Assert.AreEqual(1, state.AllowedActions.Count);
            Assert.IsTrue(state.AllowedActions.ContainsKey(expectedAction));
            Assert.IsNull(state.AllowedActions[expectedAction]);
        }

        [TestMethod]
        public void Creating_a_new_allowedstate_object_with_action_objects_should_correctly_set_properties()
        {
            var expectedName = "Test";
            var expectedAction = new TestAction();

            var state = new AllowedState(expectedName, expectedAction);

            Assert.AreEqual(expectedName, state.Name);
            Assert.IsNotNull(state.AllowedActions);
            Assert.AreEqual(1, state.AllowedActions.Count);
            Assert.IsTrue(state.AllowedActions.ContainsKey(expectedAction.Name));
            Assert.IsNotNull(state.AllowedActions[expectedAction.Name]);
        }

        [TestMethod]
        public void Creating_a_new_allowedstate_object_with_a_list_of_action_objects_should_correctly_set_properties()
        {
            var expectedName = "Test";
            var expectedAction = new TestAction();

            var state = new AllowedState(expectedName, new List<IAllowedAction> { expectedAction });

            Assert.AreEqual(expectedName, state.Name);
            Assert.IsNotNull(state.AllowedActions);
            Assert.AreEqual(1, state.AllowedActions.Count);
            Assert.IsTrue(state.AllowedActions.ContainsKey(expectedAction.Name));
            Assert.IsNotNull(state.AllowedActions[expectedAction.Name]);
        }
    }
}
