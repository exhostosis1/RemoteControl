using Listeners;
using Moq;
using Shared.Listeners;
using Shared.Logging.Interfaces;

namespace Tests;

public class SimpleServerListenerTests : IDisposable
{
    private readonly SimpleHttpListener _listener;
    private readonly ILogger<SimpleHttpListener> _logger;
    private readonly IHttpListenerWrapper _wrapper;

    public SimpleServerListenerTests()
    {
        _wrapper = Mock.Of<IHttpListenerWrapper>();
        _logger = Mock.Of<ILogger<SimpleHttpListener>>();

        _listener = new SimpleHttpListener(_wrapper, _logger);
    }

    [Fact]
    public async void StartListenTest()
    {
        var uri = new Uri("http://localhost:148");

        _listener.StartListen(uri);

        await Task.Delay(100);

        Mock.Get(_wrapper).Verify(x => x.GetContextAsync(), Times.AtLeastOnce);
    }

    [Fact]
    public async void StopListenTest()
    {
        _listener.StopListen();

        await Task.Delay(100);

        Assert.False(_listener.IsListening);
    }

    public void Dispose()
    {
    }
}