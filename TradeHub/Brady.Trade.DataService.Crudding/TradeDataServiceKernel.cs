using BWF.Hosting.Infrastructure.Interfaces;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Trade.DataService
{
    public class TradeDataServiceKernel : IKernelManipulation
    {
        private static IKernel _kernel;

        public static IKernel Kernel { get { return _kernel; } }

        public void Apply(IKernel kernel)
        {
            _kernel = kernel;
        }
    }
}
