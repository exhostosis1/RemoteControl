using Autostart;
using ConfigProviders;
using ControlProviders;
using Logging;
using Shared;
using Shared.Config;
using Shared.ControlProviders;
using Shared.Logging.Interfaces;
using Shared.UI;
using WinFormsUI;

namespace WindowsEntryPoint;

public class RemoteControlContainer : IPlatformDependantContainer
{
    public IConfigProvider ConfigProvider { get; }
    public IAutostartService AutostartService { get; }
    public ILogger Logger { get; }
    public IUserInterface UserInterface { get; }

    public ControlFacade ControlProviders { get; }

    public RemoteControlContainer()
    {
#if DEBUG
        Logger = new TraceLogger();
#else
        Logger = new FileLogger("error.log");
#endif
        var user32Wrapper = new User32Provider(Logger);
        var audioProvider = new NAudioProvider(Logger);

        ControlProviders = new ControlFacade(audioProvider, user32Wrapper, user32Wrapper, user32Wrapper);

        ConfigProvider = new LocalFileConfigProvider(Logger);
        AutostartService = new WinRegistryAutostartService(Logger);
        UserInterface = new MainForm();
    }
}