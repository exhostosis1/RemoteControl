using Shared.Enums;
using System;

namespace Shared.DIContainer.Interfaces;

public interface IContainerBuilder
{
    IContainerBuilder Register(Type interfaceType, Type instanceType, Lifetime lifetime);
    IContainerBuilder Register<TInterface, TInstance>(Lifetime lifetime) where TInterface : class where TInstance : TInterface;
    IContainerBuilder Register(Type interfaceType, object obj);
    IContainerBuilder Register<TInterface>(TInterface obj) where TInterface : class;
    IContainerBuilder Register(Type interfaceType, Delegate function);

    IContainerBuilder Register<TInterface>(Func<TInterface> function)
        where TInterface : class;

    IContainerBuilder Register<TInterface, TInstance>(Func<TInstance> function)
        where TInterface : class where TInstance : TInterface;

    ISimpleContainer Build();
}