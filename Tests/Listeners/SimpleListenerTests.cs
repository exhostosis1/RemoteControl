using Listeners;
using Microsoft.Extensions.Logging;
using Moq;
using Shared.DataObjects.Web;
using Shared.Listener;
using Shared.Wrappers.HttpListener;

namespace UnitTests.Listeners;

public class SimpleHttpListenerTests : IDisposable
{
    private readonly SimpleHttpListener _listener;
    private readonly Mock<IHttpListener> _wrapper;

    public SimpleHttpListenerTests()
    {
        _wrapper = new Mock<IHttpListener>(MockBehavior.Strict);
        var logger = Mock.Of<ILogger>();

        _listener = new SimpleHttpListener(_wrapper.Object, logger);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void StartListenTest(bool input)
    {
        var listening = input;

        var prefixes = new Mock<IPrefixesCollection>(MockBehavior.Strict);
        prefixes.Setup(x => x.Add(It.IsAny<string>()));

        _wrapper.Setup(x => x.GetNew());
        _wrapper.SetupGet(x => x.Prefixes).Returns(prefixes.Object);
        _wrapper.Setup(x => x.Start()).Callback(() => listening = true);
        _wrapper.SetupGet(x => x.IsListening).Returns(listening);
        _wrapper.Setup(x => x.Stop()).Callback(() => listening = false);

        const string uri = "http://localhost.com";

        _listener.StartListen(new StartParameters(uri));

        Assert.True(listening);

        prefixes.Verify(x => x.Add(uri));
        _wrapper.Verify(x => x.GetNew(), Times.Once);
        _wrapper.Verify(x => x.Start(), Times.Once);
        _wrapper.Verify(x => x.Stop(), input ? Times.Once : Times.Never);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void StopListenTest(bool input)
    {
        var listening = input;

        _wrapper.Setup(x => x.Stop()).Callback(() => listening = false);
        _wrapper.SetupGet(x => x.IsListening).Returns(listening);

        _listener.StopListen();

        Assert.False(listening);

        _wrapper.Verify(x => x.Stop(), input ? Times.Once : Times.Never);
    }

    [Fact]
    public void GetContextText()
    {
        const string value1 = "path1";
        const string value2 = "path2";
        const string value3 = "path3";

        static WebContext GetContext(string path) =>
            new(new WebContextRequest(path), Mock.Of<WebContextResponse>());

        _wrapper.SetupSequence(x => x.GetContext())
            .Returns(GetContext(value1))
            .Returns(GetContext(value2))
            .Returns(GetContext(value3))
            .Throws<IndexOutOfRangeException>();

        var result = _listener.GetContext() as WebContext;
        Assert.Equal(value1, result?.WebRequest.Path);

        result = _listener.GetContext() as WebContext;
        Assert.Equal(value2, result?.WebRequest.Path);

        result = _listener.GetContext() as WebContext;
        Assert.Equal(value3, result?.WebRequest.Path);

        Assert.Throws<IndexOutOfRangeException>(() => _listener.GetContext());

        _wrapper.Verify(x => x.GetContext(), Times.Exactly(4));

    }

    [Fact]
    public async Task GetContextAsyncText()
    {
        const string value1 = "path1";
        const string value2 = "path2";
        const string value3 = "path3";

        _wrapper.SetupSequence(x => x.GetContextAsync())
            .Returns(GetContext(value1))
            .Returns(GetContext(value2))
            .Returns(GetContext(value3))
            .Throws<IndexOutOfRangeException>();

        var result = await _listener.GetContextAsync() as WebContext;
        Assert.Equal(value1, result?.WebRequest.Path);

        result = await _listener.GetContextAsync() as WebContext;
        Assert.Equal(value2, result?.WebRequest.Path);

        result = await _listener.GetContextAsync() as WebContext;
        Assert.Equal(value3, result?.WebRequest.Path);

        await Assert.ThrowsAsync<IndexOutOfRangeException>(async () => await _listener.GetContextAsync());

        _wrapper.Verify(x => x.GetContextAsync(), Times.Exactly(4));
        return;

        static Task<WebContext> GetContext(string path) =>
            Task.FromResult(new WebContext(new WebContextRequest(path), Mock.Of<WebContextResponse>()));
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}