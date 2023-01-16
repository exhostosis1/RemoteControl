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
    public IControlProvider ControlProvider { get; }
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

    public IControlProvider NewControlProvider(ILogger logger) =>
        new InputProvider(new User32Wrapper(), new User32Wrapper(), new User32Wrapper(), new NAudioWrapper(), new LogWrapper<InputProvider>(logger));

    public RemoteControlContainer()
    {
        Logger = NewLogger();
        ConfigProvider = NewConfigProvider(Logger);
        AutostartService = NewAutostartService(Logger);
        ControlProvider = NewControlProvider(Logger);
        UserInterface = NewUserInterface();
    }
}