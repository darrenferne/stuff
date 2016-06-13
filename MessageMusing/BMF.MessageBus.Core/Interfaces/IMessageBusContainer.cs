using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMF.MessageBus.Core.Interfaces
{
    public interface IMessageBusContainer
    {
        object NativeContainer { get; }

        bool IsRegistered(Type t);
        bool IsRegistered(Type t, string name);
        bool IsRegistered<T>();
        bool IsRegistered<T>(string name);

        IMessageBusContainer RegisterType(Type t);
        IMessageBusContainer RegisterType(Type t, string name);
        IMessageBusContainer RegisterType(Type from, Type to);
        IMessageBusContainer RegisterType(Type from, Type to, string name);
        IMessageBusContainer RegisterType<T>();
        IMessageBusContainer RegisterType<T>(string name);
        IMessageBusContainer RegisterType<TFrom, TTo>() where TTo : TFrom;
        IMessageBusContainer RegisterType<TFrom, TTo>(string name) where TTo : TFrom;
        IMessageBusContainer RegisterObject(Type t, object obj);
        IMessageBusContainer RegisterObject(Type t, object obj, string name);
        IMessageBusContainer RegisterObject<T>(T obj);
        IMessageBusContainer RegisterObject<T>(T obj, string name);

        object ResolveType(Type t);
        object ResolveType(Type t, string name);

        T ResolveType<T>();
        T ResolveType<T>(string name);

        IList<T> ResolveAll<T>();
        IList<object> ResolveAll(Type t);
    }
}
