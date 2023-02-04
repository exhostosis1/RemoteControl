using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shared.DIContainer;

internal class TypesRegistration: ITypesRegistration
{
    private readonly Dictionary<Type, List<TypeAndLifetime>> _types = new();
    private readonly Dictionary<Type, object> _cache = new();

    private static void AddOrUpdate(IDictionary<Type, List<TypeAndLifetime>> dict, Type interfaceType,
        Type objectType, Lifetime lifetime)
    {
        if (dict.TryGetValue(interfaceType, out var item))
        {
            if (item.Any(x => x.Type == objectType))
                throw new ArgumentException("Registrations already exists");

            item.Add(new TypeAndLifetime(objectType, lifetime));
        }
        else
        {
            dict.Add(interfaceType, new List<TypeAndLifetime> { new(objectType, lifetime) });
        }
    }

    public void RegisterType(Type interfaceType, Type instanceType, Lifetime lifetime)
    {
        AddOrUpdate(_types, interfaceType, instanceType, lifetime);
    }

    public void AddCache(object obj)
    {
        if (!_cache.TryAdd(obj.GetType(), obj))
        {
            throw new ArgumentException($"Object instance of type {obj.GetType()} already in cache");
        }
    }

    public bool TryGetResteredTypes(Type type, out IEnumerable<TypeAndLifetime>? typeAndLifetime)
    {
        var result = _types.TryGetValue(type, out var value);
        typeAndLifetime = value;
        return result;
    }

    public bool TryGetCache(Type type, out object? obj)
    {
        var result = _cache.TryGetValue(type, out var value);
        obj = value;
        return result;
    }
}