using Shared.Config;
using Shared.Logging.Interfaces;
using Shared.Server;

namespace Shared;

public interface IContainer
{
    public IAutostartService AutostartService { get; }
    public IConfigProvider ConfigProvider { get; }
    public IServer Server { get; }
    public IUserInterface UserInterface { get; }
    public ILogger Logger { get; }
}