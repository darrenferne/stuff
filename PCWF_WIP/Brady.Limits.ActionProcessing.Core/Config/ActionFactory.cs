using Ninject;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Brady.Limits.ActionProcessing.Core
{
    public class ActionFactory : IActionFactory
    {
        protected readonly IKernel _kernel;
        protected readonly ConcurrentDictionary<string, Type> _resolvedTypes;

        public ActionFactory(IKernel kernel)
        {
            _kernel = kernel;
            _resolvedTypes = new ConcurrentDictionary<string, Type>();
        }

        public IKernel Kernel => _kernel;

        public Type ResolveType(string name)
        {
            return _resolvedTypes.GetOrAdd(name.ToLower(), n =>
            {
                var actionType = typeof(IAllowedAction);

                foreach (var assembly in AppDomain.CurrentDomain
                    .GetAssemblies()
                    .OrderBy(a => a.FullName.IndexOf("brady", StringComparison.OrdinalIgnoreCase) >= 0 ? 1 : 2))
                {
                    foreach (var type in assembly
                        .GetTypes()
                        .Where(t => string.Compare(t.Name, name, true) == 0 ||
                                    string.Compare(t.FullName, name, true) == 0))
                    {
                        if (actionType.IsAssignableFrom(type))
                            return type;
                    }
                }

                return null;
            });
        }
        public IAllowedAction CreateAction(string name)
        {
            var resolvedType = ResolveType(name);

            if (resolvedType is null)
                return null;

            return CreateAction(resolvedType);
        }

        public IAllowedAction CreateAction(Type type)
        {
            if (type is null)
                return null;

            return _kernel.Get(type) as IAllowedAction;
        }
    }
}
