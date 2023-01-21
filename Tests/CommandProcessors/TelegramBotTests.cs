using Bots;
using Moq;
using Shared;
using Shared.ApiControllers;
using Shared.Config;
using Shared.DataObjects.Bot;
using Shared.DataObjects.Http;
using Shared.Listeners;
using Shared.Logging.Interfaces;

namespace Tests.CommandProcessors;

public class TelegramBotTests : IDisposable
{
    public TelegramBotTests()
    {

    }

    private class MockListener : IListener<BotContext>
    {
        public ListenerState State { get; set; } = new();

        private readonly List<IObserver<BotContext>> _requestObservers = new();

        public void StartListen(StartParameters param)
        {
            State.Listening = true;

            _requestObservers.ForEach(x => x.OnNext(new BotContext()));
        }

        public void StopListen()
        {
            State.Listening = false;
        }

        public IDisposable Subscribe(IObserver<BotContext> observer)
        {
            _requestObservers.Add(observer);
            return new Unsubscriber<BotContext>(_requestObservers, observer);
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