using Moq;
using Servers.Middleware;
using Shared.ControlProviders;
using Shared.DataObjects.Bot;
using Shared.Logging.Interfaces;

namespace Tests.Endpoints;

public class CommandExecutorTests : IDisposable
{
    private readonly ILogger<CommandsExecutor> _logger;
    private readonly IControlProvider _provider;

    public CommandExecutorTests()
    {
        _logger = Mock.Of<ILogger<CommandsExecutor>>();
        _provider = Mock.Of<IControlProvider>();
    }

    private class LocalResponse: BotContextResponse
    {
        public override void Close()
        {
            
        }
    }

    [Fact]
    public void ExecuteTest()
    {
        
    }

    public void Dispose()
    {

    }
}