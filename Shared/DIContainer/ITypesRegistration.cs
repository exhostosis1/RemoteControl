using System;
using System.Collections.Generic;
using Shared.Enums;

namespace Shared.DIContainer;

public interface ITypesRegistration
{
    public void RegisterType(Type interfaceType, Type instanceType, Lifetime lifetime);
    public void AddCache(object instance);
    public bool TryGetResteredTypes(Type type, out IEnumerable<TypeAndLifetime>? typeAndLifetime);
    public bool TryGetCache(Type type, out object? obj);
}