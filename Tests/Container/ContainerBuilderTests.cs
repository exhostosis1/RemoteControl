using Moq;
using Shared.DIContainer;
using Shared.DIContainer.Interfaces;
using Shared.Enums;

namespace UnitTests.Container;

public class ContainerBuilderTests: IDisposable
{
    private readonly ContainerBuilder _builder;
    private readonly Mock<ITypesRegistration> _registration;

    public ContainerBuilderTests()
    {
        _registration = new Mock<ITypesRegistration>(MockBehavior.Strict);
        _builder = new ContainerBuilder(_registration.Object);
    }

    [Theory]
    [InlineData(typeof(TestClassWithoutInterface), typeof(TestClassWithoutInterface))]
    [InlineData(typeof(TestClassWithoutInterface), typeof(TestDerivedClassWithoutInterface))]
    [InlineData(typeof(ITestInterface), typeof(TestClassWithInterface))]
    [InlineData(typeof(ITestInterface), typeof(TestDerivedClassWithInterfaceA))]
    [InlineData(typeof(ITestInterface), typeof(TestDerivedClassWithInterfaceB))]
    [InlineData(typeof(TestClassWithInterface), typeof(TestDerivedClassWithInterfaceA))]
    [InlineData(typeof(IGenericInterface<>), typeof(GenericA<>))]
    [InlineData(typeof(IEnumerable<ITestInterface>), typeof(ITestInterface[]))]
    public void RegisterTest(Type interfaceType, Type instanceType)
    {
        _registration.Setup(x => x.RegisterType(It.IsAny<Type>(), It.IsAny<Type>(), It.IsAny<Delegate>(), It.IsAny<Lifetime>()));

        _builder.Register(interfaceType, instanceType, Lifetime.Singleton);

        _registration.Verify(x => x.RegisterType(interfaceType, instanceType, It.IsAny<Delegate>(), Lifetime.Singleton), Times.Once);
    }

    [Fact]
    public void RegisterGenericTest()
    {
        _registration.Setup(x => x.RegisterType(It.IsAny<Type>(), It.IsAny<Type>(), It.IsAny<Delegate>(), It.IsAny<Lifetime>()));
        
        _builder.Register<TestClassWithoutInterface, TestClassWithoutInterface>(Lifetime.Singleton);
        _builder.Register<TestClassWithoutInterface, TestDerivedClassWithoutInterface>(Lifetime.Singleton);
        _builder.Register<ITestInterface, TestClassWithInterface>(Lifetime.Singleton);
        _builder.Register<ITestInterface, TestDerivedClassWithInterfaceA>(Lifetime.Singleton);
        _builder.Register<ITestInterface, TestDerivedClassWithInterfaceB>(Lifetime.Singleton);
        _builder.Register<TestClassWithInterface, TestDerivedClassWithInterfaceA>(Lifetime.Singleton);
        _builder.Register<IEnumerable<ITestInterface>, ITestInterface[]>(Lifetime.Singleton);

        _registration.Verify(x => x.RegisterType(It.IsAny<Type>(), It.IsAny<Type>(), It.IsAny<Delegate>(), Lifetime.Singleton), Times.Exactly(7));
    }

    [Theory]
    [InlineData(typeof(TestClassWithoutInterface), typeof(object))]
    [InlineData(typeof(ITestInterface), typeof(ITestInterface))]
    [InlineData(typeof(ITestInterface), typeof(TestClassWithoutInterface))]
    [InlineData(typeof(ITestInterface), typeof(TestDerivedClassWithoutInterface))]
    [InlineData(typeof(TestClassWithInterface), typeof(TestClassWithoutInterface))]
    [InlineData(typeof(IGenericInterface<>), typeof(GenericB<>))]
    [InlineData(typeof(IEnumerable<ITestInterface>), typeof(IEnumerable<TestClassWithInterface>))]
    public void RegisterFailTest(Type interfaceType, Type instanceType)
    {
        Assert.Throws<ArgumentException>(() => _builder.Register(interfaceType, instanceType, Lifetime.Singleton));
    }

    [Fact]
    public void RegisterGenericFailTest()
    {
        Assert.Throws<ArgumentException>(() =>
            _builder.Register<ITestInterface, ITestInterface>(Lifetime.Singleton));
    }

    public static IEnumerable<object[]> TestObjectData =>
        new List<object[]>
        {
            new object[] { typeof(object), new object() },
            new object[] { typeof(ITestInterface), new TestClassWithInterface() },
            new object[] { typeof(ITestInterface), new TestDerivedClassWithInterfaceA() },
            new object[] { typeof(ITestInterface), new TestDerivedClassWithInterfaceB() },
            new object[] { typeof(TestClassWithoutInterface), new TestClassWithoutInterface() },
            new object[] { typeof(TestClassWithoutInterface), new TestDerivedClassWithoutInterface() },
        };

    [Theory]
    [MemberData(nameof(TestObjectData))]
    public void RegisterObjectTests(Type type, object obj)
    {
        _registration.Setup(x => x.RegisterType(It.IsAny<Type>(), It.IsAny<Type>(), It.IsAny<Delegate>(), It.IsAny<Lifetime>()));
        _registration.Setup(x => x.AddCache(It.IsAny<object>()));

        _builder.Register(type, obj);

        var objType = obj.GetType();

        _registration.Verify(x => x.RegisterType(type, objType, null, Lifetime.Singleton), Times.Once);
        _registration.Verify(x => x.AddCache(obj), Times.Once);
    }

    [Fact]
    public void RegistareObjectGenericTests()
    {
        _registration.Setup(x => x.RegisterType(It.IsAny<Type>(), It.IsAny<Type>(), It.IsAny<Delegate>(), It.IsAny<Lifetime>()));
        _registration.Setup(x => x.AddCache(It.IsAny<object>()));

        _builder.Register<object>(new object());
        _builder.Register<ITestInterface>(new TestClassWithInterface());
        _builder.Register<ITestInterface>(new TestDerivedClassWithInterfaceA());
        _builder.Register<ITestInterface>(new TestDerivedClassWithInterfaceB());
        _builder.Register<TestClassWithoutInterface>(new TestClassWithoutInterface());
        _builder.Register<TestClassWithoutInterface>(new TestDerivedClassWithoutInterface());
        _builder.Register<IEnumerable<ITestInterface>>(new List<TestClassWithInterface>());

        _registration.Verify(x => x.RegisterType(It.IsAny<Type>(), It.IsAny<Type>(), It.IsAny<Delegate>(), It.IsAny<Lifetime>()), Times.Exactly(7));
        _registration.Verify(x => x.AddCache(It.IsAny<object>()), Times.Exactly(7));
    }

    public static IEnumerable<object[]> TestFailObjectData =>
        new List<object[]>
        {
            new object[] { typeof(ITestInterface), new TestClassWithoutInterface() },
            new object[] { typeof(TestClassWithoutInterface), new TestDerivedClassWithInterfaceA() },
            new object[] { typeof(TestClassWithoutInterface), new TestDerivedClassWithInterfaceB() },
            new object[] { typeof(IEnumerable<ITestInterface>), new TestClassWithoutInterface() },
            new object[] { typeof(ITestInterface[]), new TestDerivedClassWithoutInterface() },
        };

    [Theory]
    [MemberData(nameof(TestFailObjectData))]
    public void RegisterObjectFailTest(Type type, object obj)
    {
        Assert.Throws<ArgumentException>(() => _builder.Register(type, obj));
    }

    public void Dispose()
    {
    }
}