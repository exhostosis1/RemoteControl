using System.ComponentModel;
using Listeners;
using Moq;
using Shared.Bots.Telegram;
using Shared.Bots.Telegram.ApiObjects.Response;
using Shared.Listener;
using Shared.Logging.Interfaces;

namespace UnitTests.Listeners;

public class TelegramListenerTests : IDisposable
{
    private readonly TelegramListener _listener;
    private readonly Mock<IBotApiProvider> _wrapper;

    public TelegramListenerTests()
    {
        _wrapper = new Mock<IBotApiProvider>(MockBehavior.Strict);
        var logger = Mock.Of<ILogger<TelegramListener>>();

        _listener = new TelegramListener(_wrapper.Object, logger);
    }

    [Fact]
    public void StartAndStopListenTest()
    {
        const string uri = "localhost";
        const string apiKey = "apiKey";
        var usernames = new List<string> { "user1", "user2" };

        var param = new BotParameters(uri, apiKey, usernames);
        var startStopMre = new AutoResetEvent(false);
        var contextMre = new ManualResetEventSlim(false);

        _wrapper.SetupSequence(x => x.GetUpdatesAsync(uri, apiKey, It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new UpdateResponse { Ok = true })
            .ReturnsAsync(() => new UpdateResponse { Ok = true })
            .ReturnsAsync(() => new UpdateResponse { Ok = true })
            .ReturnsAsync(() =>
            {
                contextMre.Set();
                return new UpdateResponse { Ok = true };
            });

        _listener.PropertyChanged += Handler;
        
        void Handler(object? _, PropertyChangedEventArgs args)
        {
            if(args.PropertyName != nameof(_listener.IsListening)) return;

            if(_listener.IsListening)
            {
                startStopMre.Set();
            }
            else
            {
                startStopMre.Reset();
            }
        };

        _listener.StartListen(param);

        startStopMre.WaitOne();

        Assert.True(_listener.IsListening);

        _listener.PropertyChanged -= Handler;

        _listener.PropertyChanged += Handler2;
        
        void Handler2(object? _, PropertyChangedEventArgs args)
        {
            if (args.PropertyName != nameof(_listener.IsListening)) return;

            if (_listener.IsListening)
            {
                startStopMre.Reset();
            }
            else
            {
                startStopMre.Set();
            }
        }

        contextMre.Wait();

        _listener.StopListen();

        startStopMre.WaitOne();

        Assert.False(_listener.IsListening);

        _listener.PropertyChanged -= Handler2;

        _wrapper.Verify(x => x.GetUpdatesAsync(uri, apiKey, It.IsAny<CancellationToken>()), Times.Exactly(4));
    }

    [Fact]
    public async Task GetContextAsyncTest()
    {
        const string uri = "localhost";
        const string apiKey = "apiKey";
        var usernames = new List<string> { "user1", "user2" };

        var param = new BotParameters(uri, apiKey, usernames);

        _wrapper.SetupSequence(x => x.GetUpdatesAsync(uri, apiKey, It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new UpdateResponse { Ok = true })
            .ReturnsAsync(() => new UpdateResponse
            {
                Ok = true, Result =
                [
                    new Update
                    {
                        Message = new Message
                        {
                            Chat = new Chat
                            {
                                Id = 1,
                            },
                            From = new User
                            {
                                Username = "user1"
                            },
                            Text = "test message 1",
                            ParsedDate = DateTime.Now
                        }
                    }
                ]
            })
            .ReturnsAsync(() => new UpdateResponse
            {
                Ok = true,
                Result =
                [
                    new Update
                    {
                        Message = new Message
                        {
                            Chat = new Chat
                            {
                                Id = 1,
                            },
                            From = new User
                            {
                                Username = "user2"
                            },
                            Text = "test message 2",
                            ParsedDate = DateTime.Now
                        }
                    }
                ]
            })
            .ReturnsAsync(() => new UpdateResponse
            {
                Ok = true,
                Result =
                [
                    new Update
                    {
                        Message = new Message
                        {
                            Chat = new Chat
                            {
                                Id = 1,
                            },
                            From = new User
                            {
                                Username = "user3"
                            },
                            Text = "test message 3",
                            ParsedDate = DateTime.Now
                        }
                    }
                ]
            });

        _listener.StartListen(param);

        var context = await _listener.GetContextAsync();
        Assert.Equal(1, context.BotRequest.Id);
        Assert.Equal("test message 1", context.BotRequest.Command);

        context = await _listener.GetContextAsync();
        Assert.Equal(1, context.BotRequest.Id);
        Assert.Equal("test message 2", context.BotRequest.Command);

        _listener.StopListen();
    }

    [Fact]
    public async Task GetContextTimeoutTest()
    {
        _wrapper.Setup(x => x.GetUpdatesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                Thread.Sleep(TimeSpan.FromHours(1));
                return null!;
            });

        _listener.StartListen(new BotParameters("", "", []));

        await Assert.ThrowsAsync<OperationCanceledException>(async () =>
            await _listener.GetContextAsync(new CancellationTokenSource(TimeSpan.FromSeconds(2)).Token));
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}