using Shared.Config;
using Shared.ControlProviders;
using Shared.Logging.Interfaces;
using Shared.UI;

namespace Shared;

public interface IPlatformDependantContainer
{
    public IConfigProvider ConfigProvider { get; }
    public IAutostartService AutostartService { get; }
    public IUserInterface UserInterface { get; }
    public IControlProvider ControlProvider { get; }
    public ILogger Logger { get; }

    public ILogger NewLogger();
    public IConfigProvider NewConfigProvider(ILogger logger);
    public IAutostartService NewAutostartService(ILogger logger);
    public IUserInterface NewUserInterface();
    public IControlProvider NewControlProvider(ILogger logger);
}