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
        private object _syncLock = new object();
        private IKernel _kernel;

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

        public bool IsRegistered(Type t)
        {
            lock (_syncLock)
            {
                return _kernel.CanResolve(t) != null;
            }
        }

        public bool IsRegistered(Type t, string name)
        {
            lock (_syncLock)
            {
                return _kernel.CanResolve(t, name) != null;
            }
        }

        public bool IsRegistered<T>()
        {
            lock (_syncLock)
            {
                return _kernel.CanResolve<T>();
            }
        }

        public bool IsRegistered<T>(string name)
        {
            lock (_syncLock)
            {
                return _kernel.CanResolve<T>(name);
            }
        }

        public IMessageBusContainer RegisterObject(Type t, object obj)
        {
            lock (_syncLock)
            {
                _kernel.Bind(t).ToMethod(c => obj);
            }
            return this;
        }

        public IMessageBusContainer RegisterObject(Type t, object obj, string name)
        {
            lock (_syncLock)
            {
                _kernel.Bind(t).ToMethod(c => obj).Named(name);
            }
            return this;
        }

        public IMessageBusContainer RegisterObject<T>(T obj)
        {
            lock (_syncLock)
            {
                _kernel.Bind<T>().ToMethod(c => obj);
            }
            return this;
        }

        public IMessageBusContainer RegisterObject<T>(T obj, string name)
        {
            lock (_syncLock)
            {
                _kernel.Bind<T>().ToMethod(c => obj).Named(name);
            }
            return this;
        }

        public IMessageBusContainer RegisterType(Type t)
        {
            lock (_syncLock)
            {
                _kernel.Bind(t).ToSelf();
            }
            return this;
        }

        public IMessageBusContainer RegisterType(Type from, Type to)
        {
            lock (_syncLock)
            {
                _kernel.Bind(from).To(to);
            }
            return this;
        }

        public IMessageBusContainer RegisterType(Type t, string name)
        {
            lock (_syncLock)
            {
                _kernel.Bind(t).ToSelf().Named(name);
            }
            return this;
        }

        public IMessageBusContainer RegisterType(Type from, Type to, string name)
        {
            lock (_syncLock)
            {
                _kernel.Bind(from).To(to).Named(name);
            }
            return this;
        }

        public IMessageBusContainer RegisterType<T>()
        {
            lock (_syncLock)
            {
                _kernel.Bind<T>().ToSelf();
            }
            return this;
        }

        public IMessageBusContainer RegisterType<T>(string name)
        {
            lock (_syncLock)
            {
                _kernel.Bind<T>().ToSelf().Named(name);
            }
            return this;
        }

        public IMessageBusContainer RegisterType<TFrom, TTo>() where TTo : TFrom
        {
            lock (_syncLock)
            {
                _kernel.Bind<TFrom>().To<TTo>();
            }
            return this;
        }

        public IMessageBusContainer RegisterType<TFrom, TTo>(string name) where TTo : TFrom
        {
            lock (_syncLock)
            {
                _kernel.Bind<TFrom>().To<TTo>().Named(name);
            }
            return this;
        }

        public IList<object> ResolveAll(Type t)
        {
            lock (_syncLock)
            {
                return _kernel.GetAll(t).ToList();
            }
        }

        public IList<T> ResolveAll<T>()
        {
            lock (_syncLock)
            {
                return _kernel.GetAll<T>().ToList();
            }
        }

        public object ResolveType(Type t)
        {
            lock (_syncLock)
            {
                return _kernel.Get(t);
            }
        }

        public object ResolveType(Type t, string name)
        {
            lock (_syncLock)
            {
                return _kernel.Get(t, name);
            }
        }

        public T ResolveType<T>()
        {
            lock (_syncLock)
            {
                return _kernel.Get<T>();
            }
        }

        public T ResolveType<T>(string name)
        {
            lock (_syncLock)
            {
                return _kernel.Get<T>(name);
            }
        }
    }
}
