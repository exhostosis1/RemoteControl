using Autostart;
using ConfigProviders;
using ConsoleUI;
using ControlProviders;
using Logging;
using Shared;
using Shared.Config;
using Shared.ControlProviders;
using Shared.Logging;
using Shared.Logging.Interfaces;
using Shared.UI;

namespace LinuxEntryPoint;

public class RemoteControlContainer : IPlatformDependantContainer
{
    public IConfigProvider ConfigProvider { get; }
    public IAutostartService AutostartService { get; }
    public IUserInterface UserInterface { get; }
    public ControlFacade ControlProviders { get; }
    public ILogger Logger { get; }

    public IConfigProvider NewConfigProvider() => new LocalFileConfigProvider(new LogWrapper<LocalFileConfigProvider>(Logger));
    public IAutostartService NewAutostartService() => new DummyAutostartService(new LogWrapper<DummyAutostartService>(Logger));
    public IUserInterface NewUserInterface() => new MainConsole();
    public ControlFacade NewControlFacade()
    {
        var ydoToolProvider = new YdotoolProvider(new LogWrapper<YdotoolProvider>(Logger));
        var dummy = new DummyProvider(new LogWrapper<DummyProvider>(Logger));

        return new ControlFacade(dummy, ydoToolProvider, ydoToolProvider, dummy);
    }

    public ILogger NewLogger()    {
#if DEBUG
        return new TraceLogger();
#else
        return new FileLogger(typeof(T), "error.log");
#endif
    }

    public RemoteControlContainer()
    {
        Logger = NewLogger();
        ConfigProvider = NewConfigProvider();
        AutostartService = NewAutostartService();
        UserInterface = NewUserInterface();
        ControlProviders = NewControlFacade();
    }
}