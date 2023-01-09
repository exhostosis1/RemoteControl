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
        var user32Wrapper = new User32Provider(GetLogger(typeof(User32Provider)));
        var audioProvider = new NAudioProvider(GetLogger(typeof(NAudioProvider)));

        ControlProviders = new ControlFacade(audioProvider, user32Wrapper, user32Wrapper, user32Wrapper);

        ConfigProvider = new LocalFileConfigProvider(GetLogger(typeof(LocalFileConfigProvider)));
        AutostartService = new WinRegistryAutostartService(GetLogger(typeof(WinRegistryAutostartService)));
        UserInterface = new MainForm();
    }
}