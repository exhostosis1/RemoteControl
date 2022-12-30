using Autostart;
using ConfigProviders;
using ControlProviders;
using Logging;
using RemoteControlConsole;
using Shared;
using Shared.Config;
using Shared.ControlProviders;
using Shared.Logging.Interfaces;

namespace RemoteControlLinux;

public class RemoteControlContainer : IContainer
{
    public IConfigProvider ConfigProvider { get; }
    public IAutostartService AutostartService { get; }
    public ILogger Logger { get; }
    public IUserInterface UserInterface { get; set; }
    public ControlFacade ControlProviders { get; set; }

    public RemoteControlContainer()
    {
#if DEBUG
        Logger = new TraceLogger();
#else
            Logger = new FileLogger("error.log");
#endif
        var ydotoolWrapper = new YdotoolProvider(Logger);
        var dummyWrapper = new DummyProvider(Logger);

        ControlProviders = new ControlFacade(dummyWrapper, ydotoolWrapper, ydotoolWrapper, dummyWrapper);

        ConfigProvider = new LocalFileConfigProvider(Logger);
        AutostartService = new DummyAutostartService();
        UserInterface = new ConsoleUI();
    }
}