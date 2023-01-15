using Autostart;
using ConfigProviders;
using ControlProviders;
using ControlProviders.Wrappers;
using Logging;
using Shared;
using Shared.Config;
using Shared.ControlProviders;
using Shared.Logging;
using Shared.Logging.Interfaces;
using Shared.RegistryWrapper.Registry;
using Shared.UI;
using WinFormsUI;

namespace WindowsEntryPoint;

public class RemoteControlContainer : IPlatformDependantContainer
{
    public IConfigProvider ConfigProvider { get; }
    public IAutostartService AutostartService { get; }
    public IUserInterface UserInterface { get; }
    public IAudioControlProvider AudioProvider { get; }
    public IDisplayControlProvider DisplayProvider { get; }
    public IKeyboardControlProvider KeyboardProvider { get; }
    public IMouseControlProvider MouseProvider { get; }
    public ControlFacade ControlProviders { get; }
    public ILogger Logger { get; }
    public ILogger NewLogger()
    {
#if DEBUG
        return new TraceLogger();
#else
        return new FileLogger(Path.Combine(AppContext.BaseDirectory, "error.log"));
#endif
    }

    public IConfigProvider NewConfigProvider(ILogger logger) =>
        new LocalFileConfigProvider(Path.Combine(AppContext.BaseDirectory, "config.ini"), new LogWrapper<LocalFileConfigProvider>(logger));

    public IAutostartService NewAutostartService(ILogger logger) =>
        new WinRegistryAutostartService(new RegistryWrapper(), new LogWrapper<WinRegistryAutostartService>(logger));

    public IUserInterface NewUserInterface() => new MainForm();

    public IKeyboardControlProvider NewKeyboardProvider(ILogger logger) =>
        new User32Provider(new LogWrapper<User32Provider>(Logger));

    public IMouseControlProvider NewMouseProvider(ILogger logger) =>
        new User32Provider(new LogWrapper<User32Provider>(logger));

    public IDisplayControlProvider NewDisplayProvider(ILogger logger) =>
        new User32Provider(new LogWrapper<User32Provider>(logger));

    public IAudioControlProvider NewAudioProvider(ILogger logger) =>
        new NAudioProvider(new NAudioWrapper(), new LogWrapper<NAudioProvider>(logger));

    public RemoteControlContainer()
    {
        Logger = NewLogger();

        ConfigProvider = NewConfigProvider(Logger);
        AutostartService = NewAutostartService(Logger);

        var user32Provider = new User32Provider(new LogWrapper<User32Provider>(Logger));
        var naudioProvider = new NAudioProvider(new NAudioWrapper(), new LogWrapper<NAudioProvider>(Logger));
        
        AudioProvider = naudioProvider;
        KeyboardProvider = user32Provider;
        MouseProvider = user32Provider;
        DisplayProvider = user32Provider;

        ControlProviders = new ControlFacade(AudioProvider, KeyboardProvider, MouseProvider, DisplayProvider);

        UserInterface = NewUserInterface();
    }
}