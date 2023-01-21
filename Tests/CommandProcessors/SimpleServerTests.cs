using Moq;
using Servers;
using Shared;
using Shared.Config;
using Shared.DataObjects.Http;
using Shared.Listeners;
using Shared.Logging.Interfaces;
using Shared.Server;

namespace Tests.CommandProcessors;

public class SimpleServerTests : IDisposable
{
    private readonly SimpleServer _server;
    private readonly ILogger<SimpleServer> _logger;
    private readonly Mock<AbstractMiddleware<HttpContext>> _middlewareMock;
    private readonly Mock<IListener<HttpContext>> _listenerMock;

    private int _statusChangeCount;
    private bool _isListening;
    private ServerConfig _currentConfig;

    private readonly IDisposable? _statusUnsubscriber = null;
    private readonly IDisposable? _configUnsubscriber = null;

    public SimpleServerTests()
    {
        _middlewareMock = new Mock<AbstractMiddleware<HttpContext>>(MockBehavior.Strict);
        _listenerMock = new Mock<IListener<HttpContext>>(MockBehavior.Strict);
        var state = new ListenerState();
        _listenerMock.SetupGet(x => x.State).Returns(state);
        _logger = Mock.Of<ILogger<SimpleServer>>();

        _server = new SimpleServer(_listenerMock.Object, _middlewareMock.Object, _logger);

        _statusUnsubscriber = _server.Subscribe(new Observer<bool>(status => _statusChangeCount++));
    }

    [Fact]
    public void StartWithoutConfigTest()
    {
        _listenerMock.Setup(x => x.StartListen(It.IsAny<StartParameters>()));
        _middlewareMock.Setup(x => x.ProcessRequest(It.IsAny<HttpContext>()));

        _server.Start();

        Assert.Equal(SimpleServer.DefaultConfig, _server.CurrentConfig);

        var context = new HttpContext("localhost");
        
        _middlewareMock.Verify(x => x.ProcessRequest(context), Times.Once);

        Assert.True(_statusChangeCount == 1);

        Assert.True(_server.Working);
    }

    public void Dispose()
    {
        _statusUnsubscriber?.Dispose();
        _configUnsubscriber?.Dispose();
    }
}