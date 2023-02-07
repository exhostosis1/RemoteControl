using Shared.DIContainer;

namespace UnitTests.Container;

public class TypesRegistrationTests: IDisposable
{
    [Fact]
    public void RegisterDuplicateObjectTest()
    {
        var registration = new TypesRegistration();
        var builder = new ContainerBuilder(registration);

        var type = typeof(ITestInterface);
        var obj = new TestClassWithInterface();

        builder.Register(type, obj);

        var exception = Assert.Throws<ArgumentException>(() => builder.Register(type, obj));
        Assert.True(exception.Message == "Registrations already exists");

        exception = Assert.Throws<ArgumentException>(() => builder.Register(typeof(object), obj));
        Assert.True(exception.Message == $"Object instance of type {obj.GetType()} already in cache");
    }

    public void Dispose()
    {
    }
}