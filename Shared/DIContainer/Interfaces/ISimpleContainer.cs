using System;

namespace Shared.DIContainer.Interfaces;

public interface ISimpleContainer
{
    object GetObject(Type interfaceType);
    TInterface GetObject<TInterface>() where TInterface : class;
}