using Bots;
using Moq;
using Shared;
using Shared.ApiControllers;
using Shared.Config;
using Shared.DataObjects.Bot;
using Shared.Listeners;
using Shared.Logging.Interfaces;

namespace Tests;

public class TelegramBotTests: IDisposable
{
    public TelegramBotTests()
    {

    }

    private class MockListener : IBotListener
    {
        public bool IsListening { get; private set; }
        public event BoolEventHandler? OnStatusChange;
        public event BotEventHandler? OnRequest;

        public void StartListen(string apiUrl, string apiKey, List<string> usernames)
        {
            IsListening = true;
            OnStatusChange?.Invoke(IsListening);

            OnRequest?.Invoke(new BotContext(0, ""));
        }

        public void StopListen()
        {
            IsListening = false;
            OnStatusChange?.Invoke(IsListening);
        }
    }

    [Fact]
    public void StartStopTest()
    {
        var executor = Mock.Of<ICommandExecutor>();
        var logger = Mock.Of<ILogger<TelegramBot>>();
        var listener = new MockListener();

        var statusObserver = Mock.Of<IObserver<bool>>();
        var configObserver = Mock.Of<IObserver<BotConfig>>();

        var firstConfig = new BotConfig
        {
            ApiUri = "firstUrl",
            ApiKey = "firstKey",
            Usernames = new List<string> { "user1, user2" }
        };

        var secondConfig = new BotConfig
        {
            ApiUri = "secondUrl",
            ApiKey = "secondKey",
            Usernames = new List<string> { "user3", "user4", "user5" }
        };

        var bot = new TelegramBot(listener, executor, logger, firstConfig);

        using var statusUnsub = bot.Subscribe(statusObserver);
        using var configUnsub = bot.Subscribe(configObserver);

        Assert.Same(bot.CurrentConfig, firstConfig);
        Assert.False(bot.Working);

        bot.Start();
        Assert.True(bot.Working);

        bot.Stop();
        Assert.False(bot.Working);

        bot.Start(secondConfig);
        Assert.True(bot.Working);
        Assert.Same(bot.CurrentConfig, secondConfig);

        bot.Restart();
        Assert.True(bot.Working);
        Assert.Same(bot.CurrentConfig, secondConfig);

        bot.Restart(firstConfig);
        Assert.True(bot.Working);
        Assert.Same(bot.CurrentConfig, firstConfig);

        Mock.Get(executor).Verify(x => x.Execute(It.IsAny<string>()), Times.Exactly(4));
        Mock.Get(statusObserver).Verify(x => x.OnNext(true), Times.Exactly(4));
        Mock.Get(statusObserver).Verify(x => x.OnNext(false), Times.Exactly(3));
        Mock.Get(configObserver).Verify(x => x.OnNext(It.IsAny<BotConfig>()), Times.Exactly(2));
    }

    public void Dispose()
    {

    }
}