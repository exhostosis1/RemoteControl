using Moq;
using Servers;
using Shared.Config;
using Shared.DataObjects.Web;
using Shared.Listener;
using Shared.Server;
using System.ComponentModel;
using Microsoft.Extensions.Logging;

namespace UnitTests;

public class SimpleServerTests : IDisposable
{
    private readonly ILogger _logger;
    private readonly Mock<IWebListener> _listener;
    private readonly Mock<IWebMiddlewareChain> _chain;

    private readonly SimpleServer _server;

    public SimpleServerTests()
    {
        _chain = new Mock<IWebMiddlewareChain>(MockBehavior.Strict);
        _listener = new Mock<IWebListener>(MockBehavior.Strict);
        _logger = Mock.Of<ILogger>();

        _server = new SimpleServer(_listener.Object, _chain.Object, _logger);
        
        _listener.Setup(x => x.StopListen());
    }

    [Fact]
    public void StartWithoutConfigTest()
    {
        _listener.Setup(x => x.StartListen(It.IsAny<WebParameters>()));

        StartTestLocal();

        _listener.Verify(x => x.StartListen(It.IsAny<WebParameters>()), Times.Once);
    }

    private void StartTestLocal(WebConfig? config = null)
    {
        _listener.Setup(x => x.GetContextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                Thread.Sleep(TimeSpan.FromHours(1));
                return null!;
            });
        _chain.Setup(x => x.ChainRequest(It.IsAny<WebContext>()));

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
        Assert.Equal(_server.CurrentConfig, config ?? _server.DefaultConfig);
    }

    private const string Config1 = "http://localhost:12345/";
    private const string Config2 = "https://1.2.3.4/5678";

    [Theory]
    [InlineData(Config1)]
    [InlineData(Config2)]
    [InlineData(null)]
    public void StartWithConfigTest(string? uri)
    {
        var config = uri == null ? null : new WebConfig { Uri = new Uri(uri) };
        var parameters = new WebParameters(config?.Uri.ToString() ?? _server.CurrentConfig.Uri.ToString());

        _listener.Setup(x => x.StartListen(parameters));

        StartTestLocal(config);

        _listener.Verify(x => x.StartListen(parameters), Times.Once);
    }

    [Fact]
    public void StartErrorTest()
    {
        const string exceptionMessage = "test message";

        _listener.Setup(x => x.StartListen(It.IsAny<WebParameters>())).Throws(new Exception(exceptionMessage));

        _server.Start();

        _listener.Verify(x => x.GetContextAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public void RestartTest()
    {
        var config1 = new WebConfig
        {
            Scheme = "http",
            Host = "localhost",
            Port = 123
        };

        var config2 = new WebConfig
        {
            Scheme = "https",
            Host = "192.168.0.1",
            Port = 451
        };

        _listener.Setup(x => x.StartListen(It.IsAny<WebParameters>()));
        _listener.Setup(x => x.GetContextAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() =>
        {
            Thread.Sleep(TimeSpan.FromHours(1));
            return null!;
        });

        var mre = new AutoResetEvent(false);
        _server.PropertyChanged += Handler;
        
        void Handler(object? _, PropertyChangedEventArgs args)
        {
            if(args.PropertyName != nameof(_server.Status)) return;

            if (_server.Status)
                mre.Set();
        };

        _server.Start(config1);

        mre.WaitOne();

        Assert.True(_server.Status);
        Assert.Equal(_server.CurrentConfig, config1);

        _server.PropertyChanged -= Handler;

        _server.PropertyChanged += Handler;

        _server.Restart(config2);

        mre.WaitOne();

        _server.PropertyChanged -= Handler;

        Assert.True(_server.Status);
        Assert.Equal(_server.CurrentConfig, config2);

        _listener.Verify(x => x.StartListen(new WebParameters(config1.Uri.ToString())), Times.Once);
        _listener.Verify(x => x.StartListen(new WebParameters(config2.Uri.ToString())), Times.Once);
        _listener.Verify(x => x.GetContextAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
    }


    [Fact]
    public void ProcessRequestTest()
    {
        var context = new WebContext(new WebContextRequest("path"), Mock.Of<WebContextResponse>());
        var contextMre = new ManualResetEventSlim(false);

        _listener.Setup(x => x.StartListen(It.IsAny<WebParameters>()));
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
                contextMre.Set();
                Thread.Sleep(100);
                return context;
            })
            .ReturnsAsync(() =>
            {
                Thread.Sleep(TimeSpan.FromHours(1));
                return context;
            });
        _chain.Setup(x => x.ChainRequest(It.IsAny<WebContext>()));

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

        _listener.Verify(x => x.StartListen(It.IsAny<WebParameters>()), Times.Once);
        _listener.Verify(x => x.GetContextAsync(It.IsAny<CancellationToken>()), Times.AtLeast(4));
        _chain.Verify(x => x.ChainRequest(context), Times.AtLeast(3));
    }

    [Fact]
    public void SubscriptionTest()
    {
        _listener.Setup(x => x.StartListen(It.IsAny<WebParameters>()));
        _listener.Setup(x => x.GetContextAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() =>
        {
            Thread.Sleep(50);
            return new WebContext(new WebContextRequest("path"), Mock.Of<WebContextResponse>());
        });
        _chain.Setup(x => x.ChainRequest(It.IsAny<WebContext>()));

        var s = false;
        WebConfig? c = null;

        _server.PropertyChanged += (_, args) =>
        {
            switch (args.PropertyName)
            {
                case nameof(_server.Status):
                    s = _server.Status;
                    break;
                case nameof(_server.Config):
                    c = _server.CurrentConfig;
                    break;
                default:
                    break;
            }
        };

        var config1 = new WebConfig
        {
            Scheme = "http",
            Host = "host",
            Port = 123
        };

        var mre = new AutoResetEvent(false);
        _server.PropertyChanged += Handler;
            
        void Handler(object? _, PropertyChangedEventArgs args)
        {
            if(args.PropertyName != nameof(_server.Status)) return;

            if (_server.Status)
                mre.Set();
        };

        _server.Start(config1);

        mre.WaitOne();

        Assert.True(s);
        Assert.Equal(config1, c);

        _server.PropertyChanged -= Handler;

        _server.PropertyChanged += Handler2;

        void Handler2(object? _, PropertyChangedEventArgs args)
        {
            if (args.PropertyName != nameof(_server.Status)) return;

            if (!_server.Status)
                // ReSharper disable once AccessToDisposedClosure
                mre.Set();
        };

        _server.Stop();

        mre.WaitOne();
        
        Assert.False(s);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _server.Stop();
    }
}