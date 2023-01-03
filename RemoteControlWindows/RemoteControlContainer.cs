using Autostart;
using ConfigProviders;
using ControlProviders;
using Logging;
using RemoteControlWinForms;
using Shared;
using Shared.Config;
using Shared.ControlProviders;
using Shared.Logging.Interfaces;
using Shared.UI;

namespace RemoteControlWindows;

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
        var user32Wrapper = new User32Provider(Logger);
        var audioProvider = new NAudioProvider(Logger);

        ControlProviders = new ControlFacade(audioProvider, user32Wrapper, user32Wrapper, user32Wrapper);

        ConfigProvider = new LocalFileConfigProvider(Logger);
        AutostartService = new WinRegistryAutostartService();
        UserInterface = new WinFormsUI();
    }
}