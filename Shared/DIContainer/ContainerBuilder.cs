using Shared.DIContainer.Interfaces;
using Shared.Enums;
using System;
using System.Collections.Generic;

namespace Shared.DIContainer;

public class ContainerBuilder : IContainerBuilder
{
    private readonly ITypesRegistration _typesRegistration;

    public ContainerBuilder(ITypesRegistration? typesRegistration = null)
    {
        _typesRegistration = typesRegistration ?? new TypesRegistration();
    }

    public IContainerBuilder Register(Type interfaceType, Type instanceType, Lifetime lifetime)
    {
        if (!instanceType.IsAssignableTo(interfaceType))
        {
            if(!interfaceType.IsGenericType || !instanceType.IsGenericType || !instanceType.IsAssignableToGenericType(interfaceType))
            {
                throw new ArgumentException($"{instanceType} cannot be assigned to {interfaceType}");
            }
        }

        if (instanceType.IsInterface || instanceType.IsAbstract)
        {
            throw new ArgumentException($"{instanceType} should be instantiable type");
        }

        var constructor =
            instanceType.IsArray || (instanceType.IsGenericType &&
                                     (instanceType.GetGenericTypeDefinition() == typeof(IEnumerable<>) ||
                                      instanceType.GenericTypeArguments.Length == 0))
                ? null
                : instanceType.GetFirstConstructor().CreateDelegate();

        _typesRegistration.RegisterType(interfaceType, instanceType, constructor, lifetime);

        return this;
    }

    public IContainerBuilder Register<TInterface, TInstance>(Lifetime lifetime) where TInterface : class where TInstance : TInterface
    {
        return Register(typeof(TInterface), typeof(TInstance), lifetime);
    }

    public IContainerBuilder Register(Type interfaceType, object obj)
    {
        var objType = obj.GetType();

        if (!objType.IsAssignableTo(interfaceType))
            throw new ArgumentException($"Object of type {objType} cannot be assigned to {interfaceType}");

        _typesRegistration.RegisterType(interfaceType, objType, null, Lifetime.Singleton);
        _typesRegistration.AddCache(obj);

        return this;
    }

    public IContainerBuilder Register<TInterface>(TInterface obj) where TInterface : class
    {
        return Register(typeof(TInterface), obj);
    }

    public IContainerBuilder Register(Type interfaceType, Delegate function)
    {
        if(!function.Method.ReturnType.IsAssignableTo(interfaceType))
            throw new ArgumentException($"Function result type {function.Method.ReturnType} cannot be assigned to {interfaceType}");

        _typesRegistration.RegisterType(interfaceType, function.Method.ReturnType, function, Lifetime.Transient);

        return this;
    }

    public IContainerBuilder Register<TInterface>(Func<TInterface> function)
        where TInterface : class
    {
        return Register(typeof(TInterface), function);
    }

    public IContainerBuilder Register<TInterface, TInstance>(Func<TInstance> function)
        where TInterface : class where TInstance : TInterface
    {
        return Register(typeof(TInterface), function);
    }

    public ISimpleContainer Build()
    {
        return new SimpleContainer(_typesRegistration);
    }
}