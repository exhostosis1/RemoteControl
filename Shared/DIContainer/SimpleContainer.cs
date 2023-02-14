using Shared.DIContainer.Interfaces;
using Shared.DIContainer.Records;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("UnitTests")]
namespace Shared.DIContainer;

public class SimpleContainer : ISimpleContainer
{
    private readonly ITypesRegistration _typesRegistration;

    internal SimpleContainer(ITypesRegistration typesRegistration)
    {
        _typesRegistration = typesRegistration;
    }

    private object CreateObject(Delegate? constructor)
    {
        if (constructor == null)
            throw new NullReferenceException();

        var parameterTypes = constructor.Method.GetParameters();

        var parameters =
            parameterTypes.Where(x => x.ParameterType.Namespace != "System.Runtime.CompilerServices").Select(
                x => x.IsOptional ? x.DefaultValue : GetObject(x.ParameterType)).ToArray();

        return constructor.DynamicInvoke(parameters) ?? throw new NullReferenceException("Created object cannot be null");
    }

    private object GetFromCacheOrCreate(TypeAndLifetime type)
    {
        if (_typesRegistration.TryGetCache(type.Type, out var obj))
            return obj!;
        else
        {
            var result = CreateObject(type.Constructor);
            _typesRegistration.AddCache(result);

            return result;
        }
    }

    private object GetObject(TypeAndLifetime typeAndLifetime)
    {
        return typeAndLifetime.Lifetime switch
        {
            Lifetime.Singleton => GetFromCacheOrCreate(typeAndLifetime),
            Lifetime.Transient => CreateObject(typeAndLifetime.Constructor),
            _ => throw new ArgumentOutOfRangeException(nameof(typeAndLifetime.Lifetime))
        };
    }

    private IEnumerable<T> CreateGenericEnumerable<T>(IEnumerable<TypeAndLifetime> typesAndLifetime) where T : class =>
        typesAndLifetime.Select(typeAndLifetime => (T)GetObject(typeAndLifetime));

    private T[] CreateGenericArray<T>(IEnumerable<TypeAndLifetime> typesAndLifetime) where T : class =>
        CreateGenericEnumerable<T>(typesAndLifetime).ToArray();

    private object CreateEnumerable(Type type, IEnumerable<TypeAndLifetime> typesAndLifetime, bool array = false)
    {
        var methodName = array ? nameof(CreateGenericArray) : nameof(CreateGenericEnumerable);

        var method = GetType()
            .GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic)
            ?.MakeGenericMethod(type) ?? throw new Exception($"Method {methodName} not found");

        return method.Invoke(this, new object?[] { typesAndLifetime }) ?? throw new NullReferenceException();
    }

    public object GetObject(Type interfaceType)
    {
        if (_typesRegistration.TryGetRegisteredTypes(interfaceType, out var type))
        {
            return GetObject(type!.First());
        }
        
        if (interfaceType.IsArray && _typesRegistration.TryGetRegisteredTypes(interfaceType.GetElementType()!, out var elementType))
        {
            return CreateEnumerable(interfaceType.GetElementType()!, elementType!, true);
        }

        if (interfaceType.IsGenericType)
        {
            if (_typesRegistration.TryGetRegisteredTypes(interfaceType.GetGenericTypeDefinition(), out var items))
            {
                var typeAndLifetime = items!.First();
                return GetObject(typeAndLifetime.Type.IsGenericType
                    ? typeAndLifetime with
                    {
                        Type = typeAndLifetime.Type.MakeGenericType(interfaceType.GenericTypeArguments),
                        Constructor = typeAndLifetime.Type.MakeGenericType(interfaceType.GenericTypeArguments).GetFirstConstructor().CreateDelegate()
                    }
                    : typeAndLifetime);
            }

            if (interfaceType.GetGenericTypeDefinition() == typeof(IEnumerable<>) &&
                _typesRegistration.TryGetRegisteredTypes(interfaceType.GenericTypeArguments[0], out var genericTypes))
            {
                return CreateEnumerable(interfaceType.GenericTypeArguments[0], genericTypes!);
            }
        }

        throw new ArgumentException($"{interfaceType} is not registered");
    }

    public TInterface GetObject<TInterface>() where TInterface : class => (TInterface)GetObject(typeof(TInterface));
}