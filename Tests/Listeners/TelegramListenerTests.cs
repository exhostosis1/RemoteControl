using Listeners;
using Moq;
using Shared.Bots.Telegram;
using Shared.Bots.Telegram.ApiObjects.Response;
using Shared.Listener;
using Shared.Logging.Interfaces;
using Shared.Observable;

namespace UnitTests.Listeners;

public class TelegramListenerTests : IDisposable
{
    private readonly TelegramListener _listener;
    private readonly ILogger<TelegramListener> _logger;
    private readonly Mock<IBotApiProvider> _wrapper;

    public TelegramListenerTests()
    {
        _wrapper = new Mock<IBotApiProvider>(MockBehavior.Strict);
        _logger = Mock.Of<ILogger<TelegramListener>>();

        _listener = new TelegramListener(_wrapper.Object, _logger);
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

        var statusSub = _listener.Subscribe(new Observer<bool>(status =>
        {
            if(status)
            {
                startStopMre.Set();
            }
            else
            {
                startStopMre.Reset();
            }
        }));

        _listener.StartListen(param);

        startStopMre.WaitOne();

        Assert.True(_listener.IsListening);

        statusSub.Dispose();
        
        statusSub = _listener.Subscribe(new Observer<bool>(status =>
        {
            if(status)
            {
                startStopMre.Reset();
            }
            else
            {
                startStopMre.Set();
            }
        }));

        contextMre.Wait();

        _listener.StopListen();

        startStopMre.WaitOne();

        Assert.False(_listener.IsListening);

        statusSub.Dispose();

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
            .ReturnsAsync(() =>
            {
                return new UpdateResponse
                {
                    Ok = true, Result = new[]
                    {
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
                    }
                };
            })
            .ReturnsAsync(() =>
            {
                return new UpdateResponse
                {
                    Ok = true,
                    Result = new[]
                    {
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
                    }
                };
            })
            .ReturnsAsync(() =>
            {
                return new UpdateResponse
                {
                    Ok = true,
                    Result = new[]
                    {
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
                    }
                };
            });

        _listener.StartListen(param);

        var context = await _listener.GetContextAsync();
        Assert.True(context.BotRequest.Id == 1);
        Assert.True(context.BotRequest.Command == "test message 1");

        context = await _listener.GetContextAsync();
        Assert.True(context.BotRequest.Id == 1);
        Assert.True(context.BotRequest.Command == "test message 2");

        _listener.StopListen();
    }

    [Fact]
    public void GetContextTimeoutTest()
    {
        _wrapper.Setup(x => x.GetUpdatesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                Thread.Sleep(TimeSpan.FromHours(1));
                return null!;
            });

        _listener.StartListen(new BotParameters("", "", new List<string>()));

        Assert.ThrowsAsync<OperationCanceledException>(async () =>
            await _listener.GetContextAsync(new CancellationTokenSource(TimeSpan.FromSeconds(2)).Token));
    }


    public void Dispose()
    {
    }
}