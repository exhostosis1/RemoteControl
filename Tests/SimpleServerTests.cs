using Microsoft.Extensions.Logging;
using Moq;
using Servers;
using Servers.DataObjects;
using Servers.DataObjects.Web;
using Servers.Middleware;
using Shared.Config;
using System.ComponentModel;

namespace UnitTests;

public class SimpleServerTests : IDisposable
{
    private readonly Mock<IListener> _listener;
    private readonly Mock<IMiddleware> _middleware;

    private readonly Server _server;

    public SimpleServerTests()
    {
        _listener = new Mock<IListener>(MockBehavior.Strict);
        var logger = Mock.Of<ILogger>();
        _middleware = new Mock<IMiddleware>(MockBehavior.Strict);

        _server = new Server(ServerType.Web, _listener.Object, [_middleware.Object], logger);
        
        _listener.Setup(x => x.StopListen());
    }

    [Fact]
    public void StartWithoutConfigTest()
    {
        _listener.Setup(x => x.StartListen(It.IsAny<StartParameters>()));

        StartTestLocal();

        _listener.Verify(x => x.StartListen(It.IsAny<StartParameters>()), Times.Once);
    }

    private void StartTestLocal(ServerConfig? config = null)
    {
        _listener.Setup(x => x.GetContextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                Thread.Sleep(TimeSpan.FromHours(1));
                return null!;
            });
        _middleware.Setup(x => x.ProcessRequestAsync(It.IsAny<IContext>(), It.IsAny<RequestDelegate>()));

        var mre = new AutoResetEvent(false);

        _server.PropertyChanged += (_, args) =>
        {
            if(args.PropertyName != nameof(_server.Status)) return;

            if (_server.Status)
                mre.Set();
        };

        _server.Start(config);

        mre.WaitOne();

        Assert.True(_server.Status);

        config ??= _server.DefaultConfig;

        Assert.Equal(_server.Config.Type, config.Type);
    }

    private const string Config1 = "http://localhost:12345/";
    private const string Config2 = "https://1.2.3.4/5678";

    [Theory]
    [InlineData(Config1)]
    [InlineData(Config2)]
    [InlineData(null)]
    public void StartWithConfigTest(string? uri)
    {
        var config = uri == null ? null : new ServerConfig(ServerType.Web) { Uri = new Uri(uri) };
        var parameters = new StartParameters(config?.Uri.ToString() ?? _server.Config.Uri.ToString());

        _listener.Setup(x => x.StartListen(parameters));

        StartTestLocal(config);

        _listener.Verify(x => x.StartListen(parameters), Times.Once);
    }

    [Fact]
    public void StartErrorTest()
    {
        const string exceptionMessage = "test message";

        _listener.Setup(x => x.StartListen(It.IsAny<StartParameters>())).Throws(new Exception(exceptionMessage));

        _server.Start();

        _listener.Verify(x => x.GetContextAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public void RestartTest()
    {
        var config1 = new ServerConfig(ServerType.Web)
        {
            Scheme = "http",
            Host = "localhost",
            Port = 123
        };

        var config2 = new ServerConfig(ServerType.Web)
        {
            Scheme = "https",
            Host = "192.168.0.1",
            Port = 451
        };

        _listener.Setup(x => x.StartListen(It.IsAny<StartParameters>()));
        _listener.Setup(x => x.GetContextAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() =>
        {
            Thread.Sleep(TimeSpan.FromHours(1));
            return null!;
        });

        var mre = new AutoResetEvent(false);
        _server.PropertyChanged += Handler;

        _server.Start(config1);

        mre.WaitOne();

        Assert.True(_server.Status);
        Assert.Equal(_server.Config, config1);

        _server.PropertyChanged -= Handler;

        _server.PropertyChanged += Handler;

        _server.Restart(config2);

        mre.WaitOne();

        _server.PropertyChanged -= Handler;

        Assert.True(_server.Status);
        Assert.Equal(_server.Config, config2);

        _listener.Verify(x => x.StartListen(new StartParameters(config1.Uri.ToString(), null, null)), Times.Once);
        _listener.Verify(x => x.StartListen(new StartParameters(config2.Uri.ToString(), null, null)), Times.Once);
        _listener.Verify(x => x.GetContextAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
        return;

        void Handler(object? _, PropertyChangedEventArgs args)
        {
            if(args.PropertyName != nameof(_server.Status)) return;

            if (_server.Status)
                mre.Set();
        }
    }


    [Fact]
    public void ProcessRequestTest()
    {
        var context = new WebContext(new WebContextRequest("path"), Mock.Of<WebContextResponse>());
        var contextMre = new ManualResetEventSlim(false);

        _listener.Setup(x => x.StartListen(It.IsAny<StartParameters>()));
        _listener.SetupSequence(x => x.GetContextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                Thread.Sleep(100);
                return context;
            })
            .ReturnsAsync(() =>
            {
                Thread.Sleep(100);
                return context;
            })
            .ReturnsAsync(() =>
            {
                Thread.Sleep(100);
                return context;
            })
            .ReturnsAsync(() =>
            {
                Thread.Sleep(100);
                contextMre.Set();
                return context;
            })
            .ReturnsAsync(() =>
            {
                throw new Exception("Should not be called");
                return context;
            });
        _middleware.Setup(x => x.ProcessRequestAsync(It.IsAny<IContext>(), It.IsAny<RequestDelegate>())).Returns(Task.CompletedTask);

        var startStopMre = new AutoResetEvent(false);
        _server.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName != nameof(_server.Status)) return;

            if (_server.Status)
                startStopMre.Set();
        };

        _server.Start();

        startStopMre.WaitOne();

        Assert.True(_server.Status);

        contextMre.Wait();

        _server.Stop();

        _listener.Verify(x => x.StartListen(It.IsAny<StartParameters>()), Times.Once);
        _listener.Verify(x => x.GetContextAsync(It.IsAny<CancellationToken>()), Times.AtLeast(4));
        _middleware.Verify(x => x.ProcessRequestAsync(context, It.IsAny<RequestDelegate>()), Times.AtLeast(3));
    }

    [Fact]
    public void SubscriptionTest()
    {
        _listener.Setup(x => x.StartListen(It.IsAny<StartParameters>()));
        _listener.Setup(x => x.GetContextAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() =>
        {
            Thread.Sleep(50);
            return new WebContext(new WebContextRequest("path"), Mock.Of<WebContextResponse>());
        });
        _middleware.Setup(x => x.ProcessRequestAsync(It.IsAny<IContext>(), It.IsAny<RequestDelegate>()));

        var s = false;
        ServerConfig? c = null;

        _server.PropertyChanged += (_, args) =>
        {
            switch (args.PropertyName)
            {
                case nameof(_server.Status):
                    s = _server.Status;
                    break;
                case nameof(_server.Config):
                    c = _server.Config;
                    break;
                default:
                    break;
            }
        };

        var config1 = new ServerConfig(ServerType.Web)
        {
            Scheme = "http",
            Host = "host",
            Port = 123
        };

        var mre = new AutoResetEvent(false);
        _server.PropertyChanged += Handler;

        _server.Start(config1);

        mre.WaitOne();

        Assert.True(s);
        Assert.Equal(config1, c);

        _server.PropertyChanged -= Handler;

        _server.PropertyChanged += Handler2;

        _server.Stop();

        mre.WaitOne();
        
        Assert.False(s);
        return;

        void Handler2(object? _, PropertyChangedEventArgs args)
        {
            if (args.PropertyName != nameof(_server.Status)) return;

            if (!_server.Status)
                // ReSharper disable once AccessToDisposedClosure
                mre.Set();
        }

        void Handler(object? _, PropertyChangedEventArgs args)
        {
            if(args.PropertyName != nameof(_server.Status)) return;

            if (_server.Status)
                mre.Set();
        }
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _server.Stop();
    }
}