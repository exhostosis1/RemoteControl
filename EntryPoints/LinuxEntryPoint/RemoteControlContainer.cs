using Autostart;
using ConfigProviders;
using ConsoleUI;
using ControlProviders;
using Logging;
using Shared;
using Shared.Config;
using Shared.ControlProviders;
using Shared.Logging.Interfaces;
using Shared.UI;

namespace LinuxEntryPoint;

public class RemoteControlContainer : IPlatformDependantContainer
{
    public IConfigProvider ConfigProvider { get; }
    public IAutostartService AutostartService { get; }
    public IUserInterface UserInterface { get; }
    public ControlFacade ControlProviders { get; }

    public ILogger GetLogger(Type type)
    {
#if DEBUG
        return new TraceLogger(type);
#else
        return new FileLogger(type, "error.log");
#endif
    }

    public RemoteControlContainer()
    {

        var ydotoolWrapper = new YdotoolProvider(GetLogger(typeof(YdotoolProvider)));
        var dummyWrapper = new DummyProvider(GetLogger(typeof(DummyProvider)));

        ControlProviders = new ControlFacade(dummyWrapper, ydotoolWrapper, ydotoolWrapper, dummyWrapper);

        ConfigProvider = new LocalFileConfigProvider(GetLogger(typeof(LocalFileConfigProvider)));
        AutostartService = new DummyAutostartService(GetLogger(typeof(DummyAutostartService)));
        UserInterface = new MainConsole();
    }
}