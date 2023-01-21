using Listeners;
using Moq;
using Shared.Listeners;
using Shared.Logging.Interfaces;

namespace Tests.Listeners;

public class ActiveBotListenerTests : IDisposable
{
    private readonly ActiveBotListener _listener;
    private readonly ILogger<ActiveBotListener> _logger;
    private readonly IActiveApiWrapper _wrapper;

    public ActiveBotListenerTests()
    {
        _wrapper = Mock.Of<IActiveApiWrapper>();
        _logger = Mock.Of<ILogger<ActiveBotListener>>();

        _listener = new ActiveBotListener(_wrapper, _logger);
    }

    [Fact]
    public async void StartListenTest()
    {
        var apiUrl = "apiUrl";
        var apiKey = "apiKey";
        var userNames = new List<string> { "user1", "user2" };

        _listener.StartListen(new StartParameters(new Uri(apiUrl), apiKey, userNames));

        await Task.Delay(100);

        Assert.True(_listener.State.Listening);
    }

    [Fact]
    public async void StopListenTest()
    {
        _listener.StopListen();

        await Task.Delay(100);

        Assert.False(_listener.State.Listening);
    }

    public void Dispose()
    {
    }
}