using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Listeners;
using Moq;
using Shared;
using Shared.Listeners;
using Shared.Logging.Interfaces;

namespace Tests;

public class ActiveBotListenerTests: IDisposable
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
    public void StartListenTest()
    {
        var apiUrl = "apiUrl";
        var apiKey = "apiKey";
        var userNames = new List<string>{ "user1", "user2" };

        _listener.StartListen(apiUrl, apiKey, userNames);

       /// Assert.Raises<BotEventHandler>()
    }

    public void Dispose()
    {
    }
}