using BWF.Hosting.Infrastructure.Interfaces;
using Ninject;

namespace DataServiceDesigner.DataService
{
    public class KernelManipulations : IKernelManipulation
    {
        public void Apply(IKernel kernel)
        {
            kernel.Bind<ISchemaRepository>()
                .To<SchemaRepository>()
                .InSingletonScope();
        }
    }
}
