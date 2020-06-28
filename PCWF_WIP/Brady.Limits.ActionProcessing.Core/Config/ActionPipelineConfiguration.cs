using System;
using System.Collections.Generic;
using System.Linq;

namespace Brady.Limits.ActionProcessing.Core
{
    public class ActionPipelineConfiguration : IActionPipelineConfiguration
    {
        public ActionPipelineConfiguration(IActionFactory actionFactory, string name, params AllowedState[] states)
        {
            var factoryRequired = states?
                .SelectMany(s => s.AllowedActions)
                .Any(a => a.Value is null) ?? false;

            if (factoryRequired && actionFactory is null)
                throw new ActionProcessorException("An action factory is require when state actions are defined by name only");

            Name = name;

            AllowedStates = states?
                .ToDictionary(s => s.Name);

            ActionTypes = new Dictionary<string, Type>();

            foreach (var stateAction in states?.SelectMany(s => s.AllowedActions.Select(a => new { State = s, ActionName = a.Key, Action = a.Value })))
            {
                var actionType = stateAction.Action?.GetType() ?? actionFactory.ResolveType(stateAction.ActionName);
                if (actionType is null)
                    throw new ActionProcessorException($"Failed to resolve the type for the action '{stateAction.ActionName}' defined for state '{stateAction.State}'");

                if (ActionTypes.ContainsKey(stateAction.ActionName) && actionType != ActionTypes[stateAction.ActionName])
                    throw new ActionProcessorException($"The type for the action '{stateAction.ActionName}' defined for state '{stateAction.State}' clashes with an action of the same name defined for a different state");

                if (!ActionTypes.ContainsKey(stateAction.ActionName))
                    ActionTypes.Add(stateAction.ActionName, actionType);
            }
            
            ActionFactory = actionFactory;
        }

        public ActionPipelineConfiguration(string name, params AllowedState[] states)
            : this(null, name, states)
        { }

        public ActionPipelineConfiguration(IActionFactory actionFactory, params AllowedState[] states)
            : this(actionFactory, string.Empty,  states)
        { }

        public ActionPipelineConfiguration(params AllowedState[] states)
            : this(null, string.Empty, states)
        { }

        public string Name { get; set; }
        public Dictionary<string, AllowedState> AllowedStates { get; }
        public Dictionary<string, Type> ActionTypes { get; }
        public IActionFactory ActionFactory { get; }

        private IAllowedAction DoActionFactoryFunc(Func<IActionFactory, IAllowedAction> func)
        {
            if (ActionFactory is null)
                throw new ActionProcessorException("Action Factory required. If action creation is required please supply a suitable implementation of IActionFactory via a suitable contructor");

            return func.Invoke(ActionFactory);
        }
        public IAllowedAction CreateAction(string actionName)
        {
            return DoActionFactoryFunc(f => f.CreateAction(actionName));
        }

        public IAllowedAction CreateAction(Type actionType)
        {
            return DoActionFactoryFunc(f => f.CreateAction(actionType));
        }
    }
}
