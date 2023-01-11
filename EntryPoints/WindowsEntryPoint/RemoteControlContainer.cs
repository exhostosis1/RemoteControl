using Autostart;
using ConfigProviders;
using ControlProviders;
using Logging;
using Shared;
using Shared.Config;
using Shared.ControlProviders;
using Shared.Logging;
using Shared.Logging.Interfaces;
using Shared.UI;
using WinFormsUI;

namespace WindowsEntryPoint;

public class RemoteControlContainer : IPlatformDependantContainer
{
    public IConfigProvider ConfigProvider { get; }
    public IAutostartService AutostartService { get; }
    public IUserInterface UserInterface { get; }

    public ControlFacade ControlProviders { get; }
    public ILogger Logger => GetLogger();

    public ILogger GetLogger()
    {
#if DEBUG
        return new TraceLogger();
#else
        return new FileLogger(type, "error.log");
#endif
    }

    public RemoteControlContainer()
    {
        var user32Wrapper = new User32Provider(new LogWrapper<User32Provider>(Logger));
        var audioProvider = new NAudioProvider(new LogWrapper<NAudioProvider>(Logger));

        ControlProviders = new ControlFacade(audioProvider, user32Wrapper, user32Wrapper, user32Wrapper);

        ConfigProvider = new LocalFileConfigProvider(new LogWrapper<LocalFileConfigProvider>(Logger));
        AutostartService = new WinRegistryAutostartService(new LogWrapper<WinRegistryAutostartService>(Logger));
        UserInterface = new MainForm();
    }
}