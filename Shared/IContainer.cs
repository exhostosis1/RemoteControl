using Shared.Controllers;
using Shared.Server;
using Shared.Server.Interfaces;

namespace Shared;

public interface IContainer: IPlatformDependantContainer
{
    public ICommandExecutor CommandExecutor { get; }
    public IListener Listener { get; }
    public AbstractMiddleware Middleware { get; }
}