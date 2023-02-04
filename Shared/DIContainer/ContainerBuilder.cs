using System;
using System.Collections.Generic;
using Shared.Enums;

namespace Shared.DIContainer;

public class ContainerBuilder
{
    private readonly Dictionary<Type, List<TypeAndLifetime>> _types = new();
    private readonly Dictionary<Type, object> _cache = new();

    public ContainerBuilder Register(Type interfaceType, Type instanceType, Lifetime lifetime)
    {
        if (!instanceType.IsAssignableTo(interfaceType) && !interfaceType.IsGenericType && !instanceType.IsGenericType)
            throw new ArgumentException($"{instanceType.Name} cannot be assigned to {interfaceType.Name}");

        _types.AddOrUpdate(interfaceType, instanceType, lifetime);

        return this;
    }

    public ContainerBuilder Register<TInterface, TInstance>(Lifetime lifetime) where TInterface : class where TInstance : TInterface
    {
        return Register(typeof(TInterface), typeof(TInstance), lifetime);
    }

    public ContainerBuilder Register(Type interfaceType, object obj)
    {
        var objType = obj.GetType();

        if (!objType.IsAssignableTo(interfaceType))
            throw new ArgumentException($"Object of type {objType.Name} cannot be assigned to {interfaceType.Name}");

        _types.AddOrUpdate(interfaceType, objType, Lifetime.Singleton);
        _cache.Add(objType, obj);

        return this;
    }

    public ContainerBuilder Register<TInterface>(object obj) where TInterface : class
    {
        return Register(typeof(TInterface), obj);
    }
    public SimpleContainer Build()
    {
        return new SimpleContainer(_types, _cache);
    }
}