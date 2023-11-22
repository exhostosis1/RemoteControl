using Shared;
using Shared.DIContainer;
using Shared.DIContainer.Interfaces;
using Shared.DIContainer.Records;
using Shared.Enums;

namespace UnitTests.Container;

public class SimpleContainerTests: IDisposable
{
    private class RegistrationMock : ITypesRegistration
    {
        public bool AddCacheAllowed;
        public bool GetRegisteredTypesAllowed;
        public bool GetCacheAllowed;

        public int AddCacheCount;
        public int GetRegisteredTypesCount;
        public int GetCacheCount;

        private static int _funcCount;

        private readonly Dictionary<Type, List<TypeAndLifetime>> _types = new()
        {
            {
                typeof(object), new List<TypeAndLifetime>
                {
                    new(typeof(TestClassWithInterface), typeof(TestClassWithInterface).GetFirstConstructor().CreateDelegate(), Lifetime.Transient)
                }
            },
            {
                typeof(ITestInterface), new List<TypeAndLifetime>
                {
                    new(typeof(TestClassWithInterface), typeof(TestClassWithInterface).GetFirstConstructor().CreateDelegate(), Lifetime.Transient),
                    new(typeof(TestDerivedClassWithInterfaceA), typeof(TestDerivedClassWithInterfaceA).GetFirstConstructor().CreateDelegate(), Lifetime.Transient),
                    new(typeof(TestDerivedClassWithInterfaceB), typeof(TestDerivedClassWithInterfaceB).GetFirstConstructor().CreateDelegate(), Lifetime.Transient)
                }
            },
            {
                typeof(TestClassWithInterface), new List<TypeAndLifetime>
                {
                    new(typeof(TestClassWithInterface), typeof(TestClassWithInterface).GetFirstConstructor().CreateDelegate(), Lifetime.Transient)
                }
            },
            {
                typeof(TestClassWithoutInterface), new List<TypeAndLifetime>
                {
                    new(typeof(TestDerivedClassWithoutInterface), typeof(TestDerivedClassWithoutInterface).GetFirstConstructor().CreateDelegate(), Lifetime.Singleton)
                }
            },
            {
                typeof(IGenericInterface<>), new List<TypeAndLifetime>
                {
                    new(typeof(GenericA<>), null, Lifetime.Transient)
                }
            },
            {
                typeof(TestDerivedClassWithoutInterface), new List<TypeAndLifetime>
                {
                    new(typeof(TestDerivedClassWithoutInterface), () => new TestDerivedClassWithoutInterface(_funcCount++), Lifetime.Transient)
                }
        }
        };

        private readonly Dictionary<Type, object> _cache = [];

        public void RegisterType(Type interfaceType, Type instanceType, Delegate? function, Lifetime lifetime)
        {
            throw new NotImplementedException();
        }

        public void AddCache(object instance)
        {
            if (!AddCacheAllowed)
                throw new InvalidOperationException("AddCache invocation is not allowed");

            AddCacheCount++;
            _cache.Add(instance.GetType(), instance);
        }

        public bool TryGetRegisteredTypes(Type type, out List<TypeAndLifetime>? typeAndLifetime)
        {
            if (!GetRegisteredTypesAllowed)
                throw
                    new InvalidOperationException("GetRegisteredTypes invocation is not allowed");

            GetRegisteredTypesCount++;
            return _types.TryGetValue(type, out typeAndLifetime);
        }

        public bool TryGetCache(Type type, out object? obj)
        {
            if (!GetCacheAllowed)
                throw new InvalidOperationException("GetCache invocation is not allowed");

            GetCacheCount++;
            return _cache.TryGetValue(type, out obj);
        }
    }

    private readonly SimpleContainer _container;
    private readonly RegistrationMock _registration = new();

    public SimpleContainerTests()
    {
        _container = new SimpleContainer(_registration);
    }

    [Theory]
    [InlineData(typeof(object))]
    [InlineData(typeof(ITestInterface))]
    [InlineData(typeof(TestClassWithInterface))]
    public void GetObjectTest(Type type)
    {
        _registration.GetRegisteredTypesAllowed = true;

        var result = _container.GetObject(type);

        Assert.NotNull(result);
        Assert.IsAssignableFrom(type, result);
        Assert.True(_registration.GetRegisteredTypesCount == 1);
    }

    [Fact]
    public void GetGenericObjectTest()
    {
        _registration.GetRegisteredTypesAllowed = true;

        var result = _container.GetObject<object>();

        Assert.NotNull(result);
        Assert.IsAssignableFrom<object>(result);
        Assert.True(_registration.GetRegisteredTypesCount == 1);

        result = _container.GetObject<ITestInterface>();

        Assert.NotNull(result);
        Assert.IsAssignableFrom<ITestInterface>(result);
        Assert.True(_registration.GetRegisteredTypesCount == 2);

        result = _container.GetObject<TestClassWithInterface>();

        Assert.NotNull(result);
        Assert.IsAssignableFrom<TestClassWithInterface>(result);
        Assert.True(_registration.GetRegisteredTypesCount == 3);
    }

    [Fact]
    public void GetObjectFailTest()
    {
        _registration.GetRegisteredTypesAllowed = true;

        var exception = Assert.Throws<ArgumentException>(() => _container.GetObject<TestDerivedClassWithInterfaceA>());
        Assert.True(exception.Message == $"{ typeof(TestDerivedClassWithInterfaceA) } is not registered");
    }

    [Fact]
    public void GetTransientObjectTest()
    {
        _registration.GetRegisteredTypesAllowed = true;

        var object1 = _container.GetObject<object>();
        var object2 = _container.GetObject<object>();
        var object3 = _container.GetObject<object>();

        Assert.NotSame(object1, object2);
        Assert.NotSame(object2, object3);

        Assert.True(_registration.GetRegisteredTypesCount == 3);
    }

    [Fact]
    public void GetSingletonObjectTest()
    {
        _registration.GetRegisteredTypesAllowed = true;
        _registration.AddCacheAllowed = true;
        _registration.GetCacheAllowed = true;

        var object1 = _container.GetObject<TestClassWithoutInterface>();
        var object2 = _container.GetObject<TestClassWithoutInterface>();
        var object3 = _container.GetObject<TestClassWithoutInterface>();

        Assert.Same(object1, object2);
        Assert.Same(object2, object3);

        Assert.True(_registration.GetRegisteredTypesCount == 3);
        Assert.True(_registration.GetCacheCount == 3);
        Assert.True(_registration.AddCacheCount == 1);
    }

    [Fact]
    public void GetEnumerableTest()
    {
        _registration.GetRegisteredTypesAllowed = true;

        var result = _container.GetObject<IEnumerable<ITestInterface>>();

        Assert.IsAssignableFrom<IEnumerable<ITestInterface>>(result);
        Assert.True(result.Count() == 3);
        Assert.True(_registration.GetRegisteredTypesCount == 3);
    }

    [Theory]
    [InlineData(typeof(object))]
    [InlineData(typeof(TestClassWithInterface))]
    [InlineData(typeof(TestClassWithoutInterface))]
    public void GenericWrapperTest(Type type)
    {
        _registration.GetRegisteredTypesAllowed = true;

        var result = _container.GetObject(typeof(IGenericInterface<>).MakeGenericType(type));

        Assert.IsType(typeof(GenericA<>).MakeGenericType(type), result);
        Assert.True(_registration.GetRegisteredTypesCount == 2);
    }

    [Fact]
    public void GetObjectByFunctionTest()
    {
        _registration.GetRegisteredTypesAllowed = true;

        var object1 = _container.GetObject<TestDerivedClassWithoutInterface>();
        var object2 = _container.GetObject<TestDerivedClassWithoutInterface>();
        var object3 = _container.GetObject<TestDerivedClassWithoutInterface>();

        Assert.NotNull(object1);
        Assert.IsType<TestDerivedClassWithoutInterface>(object1);
        Assert.True(object1.A == 0);

        Assert.NotNull(object2);
        Assert.IsType<TestDerivedClassWithoutInterface>(object2);
        Assert.True(object2.A == 1);

        Assert.NotNull(object3);
        Assert.IsType<TestDerivedClassWithoutInterface>(object3);
        Assert.True(object3.A == 2);

        Assert.True(_registration.GetRegisteredTypesCount == 3);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}