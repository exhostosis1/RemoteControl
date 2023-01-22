using Shared.Config;
using Shared.ControlProviders.Provider;
using Shared.Logging.Interfaces;
using Shared.UI;

namespace Shared;

public interface IPlatformDependantContainer
{
    public IConfigProvider ConfigProvider { get; }
    public IAutostartService AutostartService { get; }
    public IUserInterface UserInterface { get; }
    public IGeneralControlProvider ControlProvider { get; }
    public ILogger Logger { get; }

    public ILogger NewLogger();
    public IConfigProvider NewConfigProvider(ILogger logger);
    public IAutostartService NewAutostartService(ILogger logger);
    public IUserInterface NewUserInterface();
    public IGeneralControlProvider NewControlProvider(ILogger logger);
}