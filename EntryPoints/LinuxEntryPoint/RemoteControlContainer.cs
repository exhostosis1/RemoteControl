using Autostart;
using ConfigProviders;
using ConsoleUI;
using ControlProviders;
using ControlProviders.Wrappers;
using Logging;
using Shared;
using Shared.Config;
using Shared.ConsoleWrapper;
using Shared.ControlProviders.Input;
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
    public IKeyboardInput KeyboardInput { get; }
    public IMouseInput MouseInput { get; }
    public IDisplayInput DisplayInput { get; }
    public IAudioInput AudioInput { get; }
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

    public IGeneralControlProvider NewControlProvider(ILogger logger) =>
        new InputProvider(KeyboardInput, MouseInput, DisplayInput, AudioInput,
            new LogWrapper<InputProvider>(logger));

    public IKeyboardInput NewKeyboardInput() => new YdoToolWrapper();

    public IMouseInput NewMouseInput() => new YdoToolWrapper();

    public IDisplayInput NewDisplayInput() => new DummyWrapper();

    public IAudioInput NewAudioInput() => new DummyWrapper();

    public RemoteControlContainer()
    {
        Logger = NewLogger();
        ControlProvider = NewControlProvider(Logger);
        ConfigProvider = NewConfigProvider(Logger);
        AutostartService = NewAutostartService(Logger);
        UserInterface = NewUserInterface();

        var ydoToolWrapper = new YdoToolWrapper();
        var dummyWrapper = new DummyWrapper();

        KeyboardInput = ydoToolWrapper;
        MouseInput = ydoToolWrapper;
        AudioInput = dummyWrapper;
        DisplayInput = dummyWrapper;
    }
}