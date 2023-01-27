using Moq;
using Servers;
using Shared;
using Shared.Config;
using Shared.DataObjects.Web;
using Shared.Logging.Interfaces;
using Shared.Server;

namespace UnitTests;

public class SimpleServerTests : IDisposable
{
    private readonly ILogger<SimpleServer> _logger;
    private readonly Mock<IWebMiddleware> _middleware;
    private readonly Mock<IWebListener> _listener;

    private readonly SimpleServer _server;

    public SimpleServerTests()
    {
        _middleware = new Mock<IWebMiddleware>(MockBehavior.Strict);
        _listener = new Mock<IWebListener>(MockBehavior.Strict);
        _logger = Mock.Of<ILogger<SimpleServer>>();

        _server = new SimpleServer(_listener.Object, _middleware.Object, _logger);
        
        _listener.Setup(x => x.StopListen());
    }

    [Fact]
    public async Task StartWithoutConfigTest()
    {
        _listener.Setup(x => x.StartListen(It.IsAny<WebParameters>()));

        await StartTestLocal();

        _listener.Verify(x => x.StartListen(It.IsAny<WebParameters>()), Times.Once);
    }

    private async Task StartTestLocal(WebConfig? config = null)
    {
        _listener.Setup(x => x.GetContextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                Thread.Sleep(TimeSpan.FromHours(1));
                return null!;
            });
        _middleware.Setup(x => x.ProcessRequest(It.IsAny<WebContext>()));
        
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        using var sub = _server.Status.Subscribe(new Observer<bool>(status =>
        {
            if (status)
                // ReSharper disable once AccessToDisposedClosure
                cts.Cancel();
        }));

        _server.Start(config);

        try
        {
            await Task.Delay(TimeSpan.FromSeconds(10), cts.Token);
        }
        catch (TaskCanceledException)
        {
        }

        Assert.True(_server.Status.Working);
        Assert.Equal(_server.CurrentConfig, config ?? _server.DefaultConfig);
        
        _listener.Verify(x => x.GetContextAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    private const string Config1 = "http://localhost:12345/";
    private const string Config2 = "https://1.2.3.4/5678";

    [Theory]
    [InlineData(Config1)]
    [InlineData(Config2)]
    [InlineData(null)]
    public async Task StartWithConfigTest(string? uri)
    {
        var config = uri == null ? null : new WebConfig { Uri = new Uri(uri) };
        var parameters = new WebParameters(config?.Uri.ToString() ?? _server.CurrentConfig.Uri.ToString());

        _listener.Setup(x => x.StartListen(parameters));

        await StartTestLocal(config);

        _listener.Verify(x => x.StartListen(parameters), Times.Once);
    }

    [Fact]
    public void StartErrorTest()
    {
        const string exceptionMessage = "test message";

        _listener.Setup(x => x.StartListen(It.IsAny<WebParameters>())).Throws(new Exception(exceptionMessage));

        _server.Start();

        Mock.Get(_logger).Verify(x => x.LogError(exceptionMessage), Times.Once);
        _listener.Verify(x => x.GetContextAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RestartTest()
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

        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        var sub = _server.Status.Subscribe(new Observer<bool>(status =>
        {
            if(status)
                // ReSharper disable once AccessToModifiedClosure
                // ReSharper disable once AccessToDisposedClosure
                cts.Cancel();
        }));

        _server.Start(config1);

        try
        {
            await Task.Delay(TimeSpan.FromSeconds(10), cts.Token);
        }
        catch (TaskCanceledException)
        {
        }

        Assert.True(_server.Status.Working);
        Assert.Equal(_server.CurrentConfig, config1);

        cts.Dispose();
        sub.Dispose();

        cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        sub = _server.Status.Subscribe(new Observer<bool>(status =>
        {
            if (status)
                // ReSharper disable once AccessToDisposedClosure
                cts.Cancel();
        }));

        _server.Restart(config2);

        try
        {
            await Task.Delay(TimeSpan.FromSeconds(10), cts.Token);
        }
        catch (TaskCanceledException)
        {
        }

        cts.Dispose();
        sub.Dispose();

        Assert.True(_server.Status.Working);
        Assert.Equal(_server.CurrentConfig, config2);

        _listener.Verify(x => x.StartListen(new WebParameters(config1.Uri.ToString())), Times.Once);
        _listener.Verify(x => x.StartListen(new WebParameters(config2.Uri.ToString())), Times.Once);
        _listener.Verify(x => x.GetContextAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
    }


    [Fact]
    public async Task ProcessRequestTest()
    {
        var context = new WebContext(new WebContextRequest("path"), Mock.Of<WebContextResponse>());

        _listener.Setup(x => x.StartListen(It.IsAny<WebParameters>()));
        _listener.Setup(x => x.GetContextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                Thread.Sleep(1000);
                return context;
            });
        _middleware.Setup(x => x.ProcessRequest(It.IsAny<WebContext>()));

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        using var sub = _server.Status.Subscribe(new Observer<bool>(status =>
        {
            if (status)
                // ReSharper disable once AccessToDisposedClosure
                cts.Cancel();
        }));

        _server.Start();

        try
        {
            await Task.Delay(TimeSpan.FromSeconds(10), cts.Token);
        }
        catch (TaskCanceledException)
        {
        }

        Assert.True(_server.Status.Working);

        await Task.Delay(TimeSpan.FromSeconds(10), CancellationToken.None);

        _server.Stop();

        _listener.Verify(x => x.StartListen(It.IsAny<WebParameters>()), Times.Once);
        _listener.Verify(x => x.GetContextAsync(It.IsAny<CancellationToken>()), Times.AtLeast(9));
        _middleware.Verify(x => x.ProcessRequest(context), Times.AtLeast(9));
    }

    [Fact]
    public async Task SubscribtionTest()
    {
        _listener.Setup(x => x.StartListen(It.IsAny<WebParameters>()));
        _listener.Setup(x => x.GetContextAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() =>
        {
            Thread.Sleep(50);
            return new WebContext(new WebContextRequest("path"), Mock.Of<WebContextResponse>());
        });
        _middleware.Setup(x => x.ProcessRequest(It.IsAny<WebContext>()));

        var s = false;
        WebConfig? c = null;

        using var statusSub = _server.Status.Subscribe(new Observer<bool>(status => s = status));
        using var configSub = _server.Subscribe(new Observer<WebConfig>(config => c = config));

        var config1 = new WebConfig
        {
            Scheme = "http",
            Host = "host",
            Port = 123
        };

        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        var sub = _server.Status.Subscribe(new Observer<bool>(status =>
        {
            if (status)
                // ReSharper disable once AccessToModifiedClosure
                // ReSharper disable once AccessToDisposedClosure
                cts.Cancel();
        }));

        _server.Start(config1);

        try
        {
            await Task.Delay(TimeSpan.FromSeconds(10), cts.Token);
        }
        catch (TaskCanceledException)
        {
        }

        Assert.True(s);
        Assert.Equal(config1, c);

        cts.Dispose();
        sub.Dispose();

        cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        sub = _server.Status.Subscribe(new Observer<bool>(status =>
        {
            if (!status)
                // ReSharper disable once AccessToDisposedClosure
                cts.Cancel();
        }));

        _server.Stop();

        try
        {
            await Task.Delay(TimeSpan.FromSeconds(10), cts.Token);
        }
        catch (TaskCanceledException)
        {
        }

        cts.Dispose();
        sub.Dispose();
        
        Assert.False(s);
    }

    public void Dispose()
    {
        _server.Stop();
    }
}