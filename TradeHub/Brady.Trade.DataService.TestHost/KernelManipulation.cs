using BWF.Hosting.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using Brady.Trade.DataService.Core.Interfaces;
using Brady.Trade.DataService.Core.Concrete;

namespace Brady.Trade.DataService.TestHost
{
    public class KernelManipulation : IKernelManipulation
    {
        public void Apply(IKernel kernel)
        {
            kernel.Bind<ITradeDataServiceSettings>().To<TradeDataServiceSettings>().InSingletonScope();
        }
    }
}
