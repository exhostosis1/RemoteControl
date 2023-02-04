using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Shared.Enums;

namespace Shared.DIContainer;

public record TypeAndLifetime(Type Type, Lifetime Lifetime);

public class SimpleContainer
{
    private readonly ITypesRegistration _typesRegistration;

    internal SimpleContainer(ITypesRegistration typesRegistration)
    {
        _typesRegistration = typesRegistration;
    }

    private object CreateObject(Type type)
    {
        var constructor = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public)
            .MinBy(x => x.GetParameters().Length);

        if (constructor == null)
            throw new ArgumentException($"{type.Name} doesn't have a public non-static constructor");

        var parameterTypes = constructor.GetParameters();
        var parameters =
            parameterTypes.Select(
                x => x.IsOptional ? x.DefaultValue : GetObject(x.ParameterType)).ToArray();

        return constructor.Invoke(parameters);
    }

    private object GetFromCacheOrCreate(Type type)
    {
        if (_typesRegistration.TryGetCache(type, out var obj))
            return obj!;
        else
        {
            var result = CreateObject(type);
            _typesRegistration.AddCache(result);

            return result;
        }
    }

    private object GetObject(TypeAndLifetime typeAndLifetime)
    {
        return typeAndLifetime.Lifetime switch
        {
            Lifetime.Singleton => GetFromCacheOrCreate(typeAndLifetime.Type),
            Lifetime.Transient => CreateObject(typeAndLifetime.Type),
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
        TypeAndLifetime? typeAndLifetime;

        if (_typesRegistration.TryGetResteredTypes(interfaceType, out var type))
        {
            typeAndLifetime = type!.First();
        }
        else if (interfaceType.IsGenericType && _typesRegistration.TryGetResteredTypes(interfaceType.GetGenericTypeDefinition(), out var item))
        {
            var temp = item!.First();
            typeAndLifetime = temp.Type.IsGenericType ? temp with { Type = temp.Type.MakeGenericType(interfaceType.GenericTypeArguments) } : temp;
        }
        else if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IEnumerable<>) && _typesRegistration.TryGetResteredTypes(interfaceType.GenericTypeArguments[0], out var temp))
        {
            return CreateEnumerable(interfaceType.GenericTypeArguments[0], temp!);
        }
        else if (interfaceType.IsArray && _typesRegistration.TryGetResteredTypes(interfaceType.GetElementType()!, out var elementType))
        {
            return CreateEnumerable(interfaceType.GetElementType()!, elementType!, true);
        }
        else
        {
            throw new ArgumentException($"{interfaceType.Name} is not registered");
        }

        return GetObject(typeAndLifetime);
    }

    public TInterface GetObject<TInterface>() where TInterface : class => (TInterface)GetObject(typeof(TInterface));
}