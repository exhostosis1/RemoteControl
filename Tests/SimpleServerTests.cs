using Moq;
using Servers;
using Shared.Config;
using Shared.DataObjects.Http;
using Shared.Listeners;
using Shared.Logging.Interfaces;
using Shared.Server;

namespace Tests;

public class SimpleServerTests: IDisposable
{
    public SimpleServerTests()
    {

    }

    private class MockListener : IHttpListener
    {
        public bool IsListening { get; private set; }
        public event EventHandler<bool>? OnStatusChange;
        public event EventHandler<Context>? OnRequest;

        public void StartListen(Uri uri)
        {
            IsListening = true;
            OnStatusChange?.Invoke(this, IsListening);

            OnRequest?.Invoke(this, new Context("path"));
        }

        public void StopListen()
        {
            IsListening = false;
            OnStatusChange?.Invoke(this, IsListening);
        }
    }

    private class MockMiddleware : AbstractMiddleware
    {
        public int InvokeCount = 0;

        public override void ProcessRequest(object? sender, Context context)
        {
            InvokeCount++;
        }
    }

    [Fact]
    public void StartStopTest()
    {
        var middleware = new MockMiddleware();
        var logger = Mock.Of<ILogger<SimpleServer>>();
        var listener = new MockListener();

        var statusObserver = Mock.Of<IObserver<bool>>();
        var configObserver = Mock.Of<IObserver<ServerConfig>>();

        var firstConfig = new ServerConfig
        {
            Scheme = "http",
            Host = "localhost",
            Port = 1234
        };

        var secondConfig = new ServerConfig
        {
            Scheme = "https",
            Host = "1.2.3.4",
            Port = 5778
        };

        var server = new SimpleServer(listener, middleware, logger, firstConfig);

        using var statusUnsub = server.Subscribe(statusObserver);
        using var configUnsub = server.Subscribe(configObserver);

        Assert.Same(server.CurrentConfig, firstConfig);
        Assert.False(server.Working);

        server.Start();
        Assert.True(server.Working);

        server.Stop();
        Assert.False(server.Working);

        server.Start(secondConfig);
        Assert.True(server.Working);
        Assert.Same(server.CurrentConfig, secondConfig);

        server.Restart();
        Assert.True(server.Working);
        Assert.Same(server.CurrentConfig, secondConfig);

        server.Restart(firstConfig);
        Assert.True(server.Working);
        Assert.Same(server.CurrentConfig, firstConfig);

        Assert.True(middleware.InvokeCount == 4);
        Mock.Get(statusObserver).Verify(x => x.OnNext(true), Times.Exactly(4));
        Mock.Get(statusObserver).Verify(x => x.OnNext(false), Times.Exactly(3));
        Mock.Get(configObserver).Verify(x => x.OnNext(It.IsAny<ServerConfig>()), Times.Exactly(2));
    }

    public void Dispose()
    {

    }
}