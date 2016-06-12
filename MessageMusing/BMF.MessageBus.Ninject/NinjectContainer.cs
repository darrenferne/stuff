using BMF.MessageBus.Core.Interfaces;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMF.MessageBus.Core
{
    public class NinjectContainer : IMessageBusContainer
    {
        IKernel _kernel;

        public NinjectContainer()
        {
            _kernel = new StandardKernel();
        }

        public NinjectContainer(IKernel kernel)
        {
            _kernel = kernel;
        }

        public object NativeContainer
        {
            get { return _kernel; }
        }
    }
}
