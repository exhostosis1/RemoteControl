using Autostart;
using ConfigProviders;
using ConsoleUI;
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

namespace LinuxEntryPoint;

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
        new DummyAutostartService(new LogWrapper<DummyAutostartService>(logger));

    public IUserInterface NewUserInterface() => new MainConsole();

    private readonly YdoToolWrapper _ydoToolWrapper = new();
    private readonly DummyWrapper _dummyWrapper = new();

    public IGeneralControlProvider NewControlProvider(ILogger logger) =>
        new InputProvider(_ydoToolWrapper, _ydoToolWrapper, _dummyWrapper, _dummyWrapper,
            new LogWrapper<InputProvider>(logger));

    public RemoteControlContainer()
    {
        Logger = NewLogger();
        ControlProvider = NewControlProvider(Logger);
        ConfigProvider = NewConfigProvider(Logger);
        AutostartService = NewAutostartService(Logger);
        UserInterface = NewUserInterface();
    }
}