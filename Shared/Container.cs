using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Shared;

public record TypeAndLifetime(Type Type, Lifetime Lifetime);
public enum Lifetime
{
    Singleton,
    Transient
}

public class Container
{
    private readonly Dictionary<Type, TypeAndLifetime> _types = new();
    private readonly Dictionary<Type, object> _cache = new();

    public Container Register(Type interfaceType, Type instanceType, Lifetime lifetime)
    {
        if (!instanceType.IsAssignableTo(interfaceType) && !interfaceType.IsGenericType && !instanceType.IsGenericType)
            throw new ArgumentException($"{instanceType.Name} cannot be assigned to {interfaceType.Name}");

        _types.Add(interfaceType, new TypeAndLifetime(instanceType, lifetime));

        return this;
    }

    public Container Register<TInterface, TInstance>(Lifetime lifetime) where TInterface : class where TInstance : TInterface
    {
        return Register(typeof(TInterface), typeof(TInstance), lifetime);
    }

    public Container Register(Type interfaceType, object obj)
    {
        var objType = obj.GetType();

        if (!objType.IsAssignableTo(interfaceType))
            throw new ArgumentException($"Object of type {objType.Name} cannot be assigned to {interfaceType.Name}");

        _types.Add(interfaceType, new TypeAndLifetime(objType, Lifetime.Singleton));
        _cache.Add(objType, obj);

        return this;
    }

    public Container Register<TInterface>(object obj) where TInterface : class
    {
        return Register(typeof(TInterface), obj);
    }

    public object CreateObject(Type type)
    {
        var constructor = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public)
            .MinBy(x => x.GetParameters().Length);

        if (constructor == null)
            throw new ArgumentException($"{type.Name} doesn't have a public non-static constructor");

        var parameterTypes = constructor.GetParameters().Select(x => x.ParameterType).ToArray();
        var parameters = new object[parameterTypes.Length];

        for(var i = 0; i < parameterTypes.Length; i++)
        {
            parameters[i] = GetObject(parameterTypes[i]);
        }

        return constructor.Invoke(parameters);
    }

    public object GetFromCacheOrCreate(Type type)
    {
        if(_cache.TryGetValue(type, out var obj))
            return obj;
        else
        {
            var result = CreateObject(type);
            _cache.Add(type, result);

            return result;
        }
    }

    public object GetObject(Type interfaceType)
    {
        TypeAndLifetime? typeAndLifetime;

        if (_types.ContainsKey(interfaceType))
        {
            typeAndLifetime = _types[interfaceType];
        }
        else if(interfaceType.IsGenericType && _types.TryGetValue(interfaceType.GetGenericTypeDefinition(), out var temp))
        {
            typeAndLifetime = temp.Type.IsGenericType ? temp with { Type = temp.Type.MakeGenericType(interfaceType.GenericTypeArguments) } : temp;
        }
        else
        {
            throw new ArgumentException($"{interfaceType.Name} is not registered");
        }

        return typeAndLifetime.Lifetime switch
        {
            Lifetime.Singleton => GetFromCacheOrCreate(typeAndLifetime.Type),
            Lifetime.Transient => CreateObject(typeAndLifetime.Type),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public TInterface GetObject<TInterface>() where TInterface : class => (TInterface)GetObject(typeof(TInterface));
}