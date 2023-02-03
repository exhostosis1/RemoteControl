using Autostart;
using ConfigProviders;
using ControlProviders.Wrappers;
using ControlProviders;
using Logging;
using Shared.Config;
using Shared.ConsoleWrapper;
using Shared.ControlProviders.Input;
using Shared.ControlProviders.Provider;
using Shared.Logging.Interfaces;
using Shared.UI;
using Shared;
using System.Runtime.InteropServices;
using ConsoleUI;

namespace LinuxEntryPoint;

public static class Program
{
    public static void Main()
    {
#if DEBUG
        ILogger logger = new TraceLogger(new TraceWrapper());
#else
        ILogger logger = new FileLogger(Path.Combine(AppContext.BaseDirectory, "error.log"));
#endif

        var container = new Container()
            .Register<ILogger>(logger)
            .Register<IConfigProvider, LocalFileConfigProvider>(Lifetime.Singleton)
            .Register<IAutostartService, DummyAutostartService>(Lifetime.Singleton)
            .Register<IGeneralControlProvider, InputProvider>(Lifetime.Singleton)
            .Register<IUserInterface, MainConsole>(Lifetime.Singleton)
            .Register<IKeyboardInput, YdoToolWrapper>(Lifetime.Singleton)
            .Register<IMouseInput, YdoToolWrapper>(Lifetime.Singleton)
            .Register<IDisplayInput, DummyWrapper>(Lifetime.Singleton)
            .Register<IAudioInput, DummyWrapper>(Lifetime.Singleton);

        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            logger.LogError(typeof(Program), "OS not supported");
            return;
        }

        if (Environment.UserName != "root")
        {
            logger.LogError(typeof(Program), "Should run as root");
            return;
        }

        RemoteControlMain.Program.Run(container);
    }
}