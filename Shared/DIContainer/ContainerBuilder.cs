using Shared.Enums;
using System;

namespace Shared.DIContainer;

public class ContainerBuilder
{
    private readonly ITypesRegistration _typesRegistration;

    public ContainerBuilder(ITypesRegistration? typesRegistration = null)
    {
        _typesRegistration = typesRegistration ?? new TypesRegistration();
    }

    public ContainerBuilder Register(Type interfaceType, Type instanceType, Lifetime lifetime)
    {
        if (!instanceType.IsAssignableTo(interfaceType) && !interfaceType.IsGenericType && !instanceType.IsGenericType)
            throw new ArgumentException($"{instanceType.Name} cannot be assigned to {interfaceType.Name}");

        _typesRegistration.RegisterType(interfaceType, instanceType, lifetime);

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

        _typesRegistration.RegisterType(interfaceType, objType, Lifetime.Singleton);
        _typesRegistration.AddCache(obj);

        return this;
    }

    public ContainerBuilder Register<TInterface>(object obj) where TInterface : class
    {
        return Register(typeof(TInterface), obj);
    }
    public SimpleContainer Build()
    {
        return new SimpleContainer(_typesRegistration);
    }
}