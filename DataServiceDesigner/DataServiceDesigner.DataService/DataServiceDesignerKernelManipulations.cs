using BWF.Hosting.Infrastructure.Interfaces;
using Ninject;

namespace DataServiceDesigner.DataService
{
    public class DataServiceDesignerKernelManipulations : IKernelManipulation
    {
        public void Apply(IKernel kernel)
        {
            kernel.Bind<ISchemaBrowserHelpers>().To<SchemaBrowerHelpers>().InSingletonScope();
        }
    }
}
