using Autostart;
using Moq;
using Shared.Logging.Interfaces;
using Shared.Wrappers.RegistryWrapper;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Tests.Autostart;

public class RegistryAutostartServiceTests : IDisposable
{
    private readonly RegistryAutostartService _service;
    private readonly Mock<IRegistryKey> _runKeyMock;
    private readonly Mock<ILogger<RegistryAutostartService>> _logger;

    private const string RegName = "Remote Control";
    private readonly string _regValue = $"\"{Process.GetCurrentProcess().MainModule?.FileName ?? throw new NullReferenceException()}\"";

    private readonly Expression<Func<IRegistryKey, object?>> _getExpression = x => x.GetValue(RegName, "");
    private readonly Expression<Action<IRegistryKey>> _deleteExpression = x => x.DeleteValue(RegName, false);
    private readonly Expression<Action<IRegistryKey>> _setExpression;

    public RegistryAutostartServiceTests()
    {
        _logger = new Mock<ILogger<RegistryAutostartService>>(MockBehavior.Loose);
        Mock<IRegistry> registryMock = new(MockBehavior.Strict);
        _runKeyMock = new Mock<IRegistryKey>(MockBehavior.Strict);

        Mock<IRegistryKey> registryKeyMock = new(MockBehavior.Strict);
        registryKeyMock.Setup(key => key.OpenSubKey(It.IsAny<string>())).Returns(registryKeyMock.Object);
        registryKeyMock.Setup(key => key.OpenSubKey("Run", true)).Returns(_runKeyMock.Object);

        registryMock.Setup(reg => reg.CurrentUser).Returns(registryKeyMock.Object);

        _service = new RegistryAutostartService(registryMock.Object, _logger.Object);

        _setExpression = x => x.SetValue(RegName, _regValue, RegValueType.String);
    }

    [Fact]
    public void CheckAutostartValueTest()
    {
        _runKeyMock.Setup(_getExpression).Returns(_regValue);

        var result = _service.CheckAutostart();
        Assert.True(result);

        _runKeyMock.Verify(_getExpression, Times.Once);
    }

    [Fact]
    public void CheckAutostartNullTest()
    {
        _runKeyMock.Setup(_getExpression).Returns(null as object);

        var result = _service.CheckAutostart();
        Assert.False(result);

        _runKeyMock.Verify(_getExpression, Times.Once);
    }

    [Fact]
    public void SetAutostartTrueTest()
    {
        var value = "initial value";

        _runKeyMock.Setup(_deleteExpression).Callback(() => value = null);
        _runKeyMock.Setup(_setExpression).Callback(() => value = _regValue);

        _service.SetAutostart(true);

        _runKeyMock.Verify(_deleteExpression, Times.Once);
        _runKeyMock.Verify(_setExpression, Times.Once);

        Assert.Equal(value, _regValue);
    }

    [Fact]
    public void SetAutostartFalseTest()
    {
        var value = "initial value";

        _runKeyMock.Setup(_deleteExpression).Callback(() => value = null);
        _runKeyMock.Setup(_setExpression).Callback(() => value = _regValue);

        _service.SetAutostart(false);

        _runKeyMock.Verify(_deleteExpression, Times.Once);
        _runKeyMock.Verify(_setExpression, Times.Never);

        Assert.Null(value);
    }

    [Fact]
    public void SetAutostartExceptionTest()
    {
        _runKeyMock.Setup(_deleteExpression);
        _runKeyMock.Setup(_setExpression)
            .Throws(new Exception("test exception"));

        _service.SetAutostart(true);

        _runKeyMock.Verify(_deleteExpression, Times.Once);
        _runKeyMock.Verify(_setExpression, Times.Once);

        Mock.Get(_logger.Object).Verify(x => x.LogError("test exception"), Times.Once);
    }

    public void Dispose()
    {
    }
}