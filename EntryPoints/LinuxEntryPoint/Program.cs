using Autostart;
using ConfigProviders;
using ConsoleUI;
using ControlProviders.Wrappers;
using Logging;
using Shared.Autostart;
using Shared.Config;
using Shared.ConsoleWrapper;
using Shared.ControlProviders.Input;
using Shared.DIContainer;
using Shared.Enums;
using Shared.Logging.Interfaces;
using Shared.UI;
using System.Runtime.InteropServices;
using RemoteControlMain;

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

        var app = new AppBuilder(new ContainerBuilder())
            .RegisterBasicDependencies()
            .RegisterDependency<ILogger>(logger)
            .RegisterDependency<IConfigProvider, LocalFileConfigProvider>(Lifetime.Singleton)
            .RegisterDependency<IAutostartService, DummyAutostartService>(Lifetime.Singleton)
            .RegisterDependency<IUserInterface, MainConsole>(Lifetime.Singleton)
            .RegisterDependency<IKeyboardInput, YdoToolWrapper>(Lifetime.Singleton)
            .RegisterDependency<IMouseInput, YdoToolWrapper>(Lifetime.Singleton)
            .RegisterDependency<IDisplayInput, DummyWrapper>(Lifetime.Singleton)
            .RegisterDependency<IAudioInput, DummyWrapper>(Lifetime.Singleton)
            .Build();

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

        app.Run();
    }
}