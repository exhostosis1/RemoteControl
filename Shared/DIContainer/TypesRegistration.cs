using Shared.DIContainer.Interfaces;
using Shared.DIContainer.Records;
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
        Type objectType, Delegate? constructor, Lifetime lifetime)
    {
        if (dict.TryGetValue(interfaceType, out var item))
        {
            if (item.Any(x => x.Type == objectType))
                throw new ArgumentException("Registrations already exists");

            item.Add(new TypeAndLifetime(objectType, constructor, lifetime));
        }
        else
        {
            dict.Add(interfaceType, new List<TypeAndLifetime> { new(objectType, constructor, lifetime) });
        }
    }

    public void RegisterType(Type interfaceType, Type instanceType, Delegate? constructor, Lifetime lifetime)
    {
        AddOrUpdate(_types, interfaceType, instanceType, constructor, lifetime);
    }

    public void AddCache(object obj)
    {
        if (!_cache.TryAdd(obj.GetType(), obj))
        {
            throw new ArgumentException($"Object instance of type {obj.GetType()} already in cache");
        }
    }

    public bool TryGetRegisteredTypes(Type type, out List<TypeAndLifetime>? typeAndLifetime) => _types.TryGetValue(type, out typeAndLifetime);

    public bool TryGetCache(Type type, out object? obj) => _cache.TryGetValue(type, out obj);
}