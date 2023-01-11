using Shared.ApiControllers;
using Shared.Listeners;
using Shared.Server;

namespace Shared;

public interface IContainer: IPlatformDependantContainer
{
    public IHttpListener HttpListener { get; }
    public IBotListener BotListener { get; }
    public AbstractMiddleware Middleware { get; }
    public ICommandExecutor Executor { get; }
}