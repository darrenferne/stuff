using BWF.Hosting.Infrastructure.Interfaces;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
