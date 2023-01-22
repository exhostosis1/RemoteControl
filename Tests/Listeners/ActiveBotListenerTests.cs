using Listeners;
using Moq;
using Shared.Bots.Telegram;
using Shared.DataObjects.Bot;
using Shared.Listeners;
using Shared.Logging.Interfaces;

namespace Tests.Listeners;

public class ActiveBotListenerTests : IDisposable
{
    private readonly IListener<BotContext> _listener;
    private readonly ILogger<TelegramListener> _logger;
    private readonly IBotApiProvider _wrapper;

    public ActiveBotListenerTests()
    {
        _wrapper = Mock.Of<IBotApiProvider>();
        _logger = Mock.Of<ILogger<TelegramListener>>();

        _listener = new TelegramListener(_wrapper, _logger);
    }

    [Fact]
    public async void StartListenTest()
    {
        var apiUrl = "apiUrl";
        var apiKey = "apiKey";
        var userNames = new List<string> { "user1", "user2" };

        _listener.StartListen(new StartParameters(apiUrl, apiKey, userNames));

        await Task.Delay(100);

        Assert.True(_listener.IsListening);
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