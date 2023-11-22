using System.Diagnostics;
using System.Linq.Expressions;
using AutoStart;
using Moq;
using Shared.Logging.Interfaces;
using Shared.Wrappers.RegistryWrapper;

namespace UnitTests.AutoStart;

public class RegistryAutoStartServiceTests : IDisposable
{
    private readonly RegistryAutoStartService _service;
    private readonly Mock<IRegistryKey> _runKeyMock;
    private readonly ILogger<RegistryAutoStartService> _logger;

    private const string RegName = "Remote Control";
    private readonly string _regValue = $"\"{Process.GetCurrentProcess().MainModule?.FileName ?? throw new NullReferenceException()}\"";

    private readonly Expression<Func<IRegistryKey, object?>> _getExpression = x => x.GetValue(RegName, "");
    private readonly Expression<Action<IRegistryKey>> _deleteExpression = x => x.DeleteValue(RegName, false);
    private readonly Expression<Action<IRegistryKey>> _setExpression;

    public RegistryAutoStartServiceTests()
    {
        _logger = Mock.Of<ILogger<RegistryAutoStartService>>();
        Mock<IRegistry> registryMock = new(MockBehavior.Strict);
        _runKeyMock = new Mock<IRegistryKey>(MockBehavior.Strict);

        Mock<IRegistryKey> registryKeyMock = new(MockBehavior.Strict);
        registryKeyMock.Setup(key => key.OpenSubKey(It.IsAny<string>())).Returns(registryKeyMock.Object);
        registryKeyMock.Setup(key => key.OpenSubKey("Run", true)).Returns(_runKeyMock.Object);

        registryMock.Setup(reg => reg.CurrentUser).Returns(registryKeyMock.Object);

        _service = new RegistryAutoStartService(registryMock.Object, _logger);

        _setExpression = x => x.SetValue(RegName, _regValue, RegValueType.String);
    }

    [Fact]
    public void CreateServiceFailTest()
    {
        var reg = new Mock<IRegistry>();
        var regKey = new Mock<IRegistryKey>();
        regKey.Setup(x => x.OpenSubKey(It.IsAny<string>(), It.IsAny<bool>())).Returns((IRegistryKey?)null);
        reg.SetupGet(x => x.CurrentUser).Returns(regKey.Object);

        Assert.Throws<NullReferenceException>(() => new RegistryAutoStartService(reg.Object, _logger));
    }

    [Fact]
    public void CheckAutoStartValueTest()
    {
        _runKeyMock.Setup(_getExpression).Returns(_regValue);

        var result = _service.CheckAutoStart();
        Assert.True(result);

        _runKeyMock.Verify(_getExpression, Times.Once);
    }

    [Fact]
    public void CheckAutoStartNullTest()
    {
        _runKeyMock.Setup(_getExpression).Returns(null as object);

        var result = _service.CheckAutoStart();
        Assert.False(result);

        _runKeyMock.Verify(_getExpression, Times.Once);
    }

    [Fact]
    public void SetAutoStartTrueTest()
    {
        var value = "initial value";

        _runKeyMock.Setup(_deleteExpression).Callback(() => value = null);
        _runKeyMock.Setup(_setExpression).Callback(() => value = _regValue);

        _service.SetAutoStart(true);

        _runKeyMock.Verify(_deleteExpression, Times.Once);
        _runKeyMock.Verify(_setExpression, Times.Once);

        Assert.Equal(value, _regValue);
    }

    [Fact]
    public void SetAutoStartFalseTest()
    {
        var value = "initial value";

        _runKeyMock.Setup(_deleteExpression).Callback(() => value = null);
        _runKeyMock.Setup(_setExpression).Callback(() => value = _regValue);

        _service.SetAutoStart(false);

        _runKeyMock.Verify(_deleteExpression, Times.Once);
        _runKeyMock.Verify(_setExpression, Times.Never);

        Assert.Null(value);
    }

    [Fact]
    public void SetAutoStartExceptionTest()
    {
        _runKeyMock.Setup(_deleteExpression);
        _runKeyMock.Setup(_setExpression)
            .Throws(new Exception("test exception"));

        _service.SetAutoStart(true);

        _runKeyMock.Verify(_deleteExpression, Times.Once);
        _runKeyMock.Verify(_setExpression, Times.Once);

        Mock.Get(_logger).Verify(x => x.LogError("test exception"), Times.Once);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}