using Autostart;
using ConfigProviders;
using ControlProviders;
using ControlProviders.Wrappers;
using Logging;
using Shared;
using Shared.Config;
using Shared.ConsoleWrapper;
using Shared.ControlProviders.Provider;
using Shared.Logging;
using Shared.Logging.Interfaces;
using Shared.UI;
using Shared.Wrappers.Registry;
using WinFormsUI;

namespace WindowsEntryPoint;

public class RemoteControlContainer : IPlatformDependantContainer
{
    public IConfigProvider ConfigProvider { get; }
    public IAutostartService AutostartService { get; }
    public IUserInterface UserInterface { get; }
    public IGeneralControlProvider ControlProvider { get; }
    public ILogger Logger { get; }
    public ILogger NewLogger()
    {
#if DEBUG
        return new TraceLogger(new TraceWrapper());
#else
        return new FileLogger(Path.Combine(AppContext.BaseDirectory, "error.log"));
#endif
    }

    public IConfigProvider NewConfigProvider(ILogger logger) =>
        new LocalFileConfigProvider(Path.Combine(AppContext.BaseDirectory, "config.ini"), new LogWrapper<LocalFileConfigProvider>(logger));

    public IAutostartService NewAutostartService(ILogger logger) =>
        new RegistryAutostartService(new RegistryWrapper(), new LogWrapper<RegistryAutostartService>(logger));

    public IUserInterface NewUserInterface() => new MainForm();

    private readonly User32Wrapper _user32Wrapper = new();
    private readonly NAudioWrapper _naudioWrapper = new();

    public IGeneralControlProvider NewControlProvider(ILogger logger) =>
        new InputProvider(_user32Wrapper, _user32Wrapper, _user32Wrapper, _naudioWrapper, new LogWrapper<InputProvider>(logger));

    public RemoteControlContainer()
    {
        Logger = NewLogger();
        ConfigProvider = NewConfigProvider(Logger);
        AutostartService = NewAutostartService(Logger);
        ControlProvider = NewControlProvider(Logger);
        UserInterface = NewUserInterface();
    }
}