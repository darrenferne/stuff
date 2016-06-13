using BMF.MessageBus.Core.Interfaces;
using Microsoft.Practices.Unity;
using NativeUnityContainer = Microsoft.Practices.Unity.UnityContainer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMF.MessageBus.Core
{
    public class UnityContainer : IMessageBusContainer
    {
        private object _syncLock = new object();
        private IUnityContainer _container;

        public UnityContainer()
        {
            _container = new NativeUnityContainer();
        }

        public UnityContainer(IUnityContainer container)
        {
            _container = container;
        }

        public object NativeContainer
        {
            get { return _container; }
        }

        public bool IsRegistered(Type t)
        {
            lock (_syncLock)
            {
                return _container.IsRegistered(t);
            }
        }
        
        public bool IsRegistered(Type t, string name)
        {
            lock (_syncLock)
            {
                return _container.IsRegistered(t, name);
            }
        }

        public bool IsRegistered<T>()
        {
            lock (_syncLock)
            {
                return _container.IsRegistered<T>();
            }
        }
        
        public bool IsRegistered<T>(string name)
        {
            lock (_syncLock)
            {
                return _container.IsRegistered<T>(name);
            }
        }
        
        public IMessageBusContainer RegisterType(Type t)
        {
            lock (_syncLock)
            {
                _container.RegisterType(t);
            }
            return this;
        }

        public IMessageBusContainer RegisterType(Type t, string name)
        {
            lock (_syncLock)
            {
                _container.RegisterType(t, name);
            }
            return this;
        }

        public IMessageBusContainer RegisterType(Type from, Type to)
        {
            lock (_syncLock)
            {
                _container.RegisterType(from, to);
            }
            return this;
        }

        public IMessageBusContainer RegisterType(Type from, Type to, string name)
        {
            lock (_syncLock)
            {
                _container.RegisterType(from, to, name);
            }
            return this;
        }

        public IMessageBusContainer RegisterType<T>()
        {
            lock (_syncLock)
            {
                _container.RegisterType<T>();
            }
            return this;
        }

        public IMessageBusContainer RegisterType<T>(string name)
        {
            lock (_syncLock)
            {
                _container.RegisterType<T>(name);
            }
            return this;
        }

        public IMessageBusContainer RegisterType<TFrom, TTo>() where TTo : TFrom
        {
            lock (_syncLock)
            {
                _container.RegisterType<TFrom, TTo>();
            }
            return this;
        }

        public IMessageBusContainer RegisterType<TFrom, TTo>(string name) where TTo : TFrom
        {
            lock (_syncLock)
            {
                _container.RegisterType<TFrom, TTo>(name);
            }
            return this;
        }

        public IMessageBusContainer RegisterObject(Type t, object obj)
        {
            lock (_syncLock)
            {
                _container.RegisterInstance(t, obj);
            }
            return this;
        }

        public IMessageBusContainer RegisterObject(Type t, object obj, string name)
        {
            lock (_syncLock)
            {
                _container.RegisterInstance(t, name, obj);
            }
            return this;
        }

        public IMessageBusContainer RegisterObject<T>(T obj)
        {
            lock (_syncLock)
            {
                _container.RegisterInstance<T>(obj);
            }
            return this;
        }

        public IMessageBusContainer RegisterObject<T>(T obj, string name)
        {
            lock (_syncLock)
            {
                _container.RegisterInstance<T>(name, obj);
            }
            return this;
        }

        public object ResolveType(Type t)
        {
            lock (_syncLock)
            {
                return _container.Resolve(t);
            }
        }
                
        public object ResolveType(Type t, string name)
        {
            lock (_syncLock)
            {
                return _container.Resolve(t, name);
            }
        }
                
        public T ResolveType<T>()
        {
            lock (_syncLock)
            {
                return _container.Resolve<T>();
            }
        }
                
        public T ResolveType<T>(string name)
        {
            lock (_syncLock)
            {
                return _container.Resolve<T>(name);
            }
        }
                
        public IList<T> ResolveAll<T>()
        {
            lock (_syncLock)
            {
                return _container.ResolveAll<T>().ToList<T>();
            }
        }

        public IList<object> ResolveAll(Type t)
        {
            lock (_syncLock)
            {
                return _container.ResolveAll(t).ToList();
            }
        }
    }
}
