using Ninject;

namespace Brady.Limits.ActionProcessing.Core.Tests
{
    public class TestActionFactory : ActionFactory
    {
        public TestActionFactory(IKernel kernel = null)
            : base(kernel ?? new StandardKernel())
        { }
    }
}
