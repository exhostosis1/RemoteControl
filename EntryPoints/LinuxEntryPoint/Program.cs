using Autostart;
using ConfigProviders;
using ConsoleUI;
using ControlProviders.Wrappers;
using Logging;
using Shared;
using Shared.Config;
using Shared.ConsoleWrapper;
using Shared.ControlProviders.Input;
using Shared.Logging.Interfaces;
using Shared.UI;
using System.Runtime.InteropServices;

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

        var container = new ContainerBuilder()
            .Register<ILogger>(logger)
            .Register<IConfigProvider, LocalFileConfigProvider>(Lifetime.Singleton)
            .Register<IAutostartService, DummyAutostartService>(Lifetime.Singleton)
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