using Autostart;
using ConfigProviders;
using ControlProviders.Wrappers;
using Logging;
using Microsoft.Win32;
using Shared.Autostart;
using Shared.Config;
using Shared.ConsoleWrapper;
using Shared.ControlProviders.Input;
using Shared.DIContainer;
using Shared.Enums;
using Shared.Logging.Interfaces;
using Shared.UI;
using Shared.Wrappers.Registry;
using Shared.Wrappers.RegistryWrapper;
using System.Runtime.InteropServices;
using RemoteControlMain;
using WinFormsUI;

namespace WindowsEntryPoint;

public static class Program
{
    public static void Main()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return;

#if DEBUG
        ILogger logger = new TraceLogger(new TraceWrapper());
#else
        ILogger logger = new FileLogger(Path.Combine(AppContext.BaseDirectory, "error.log"));
#endif

        var app = new AppBuilder(new ContainerBuilder())
            .RegisterBasicDependencies()
            .RegisterDependency<ILogger>(logger)
            .RegisterDependency<IConfigProvider, LocalFileConfigProvider>(Lifetime.Singleton)
            .RegisterDependency<IRegistry, RegistryWrapper>(Lifetime.Singleton)
            .RegisterDependency<IAutostartService, RegistryAutostartService>(Lifetime.Singleton)
            .RegisterDependency<IUserInterface, MainForm>(Lifetime.Singleton)
            .RegisterDependency<IKeyboardInput, User32Wrapper>(Lifetime.Singleton)
            .RegisterDependency<IMouseInput, User32Wrapper>(Lifetime.Singleton)
            .RegisterDependency<IDisplayInput, User32Wrapper>(Lifetime.Singleton)
            .RegisterDependency<IAudioInput, NAudioWrapper>(Lifetime.Singleton)
            .Build();
        
        var type = typeof(Program);

        var ids = new List<int>();

        SystemEvents.SessionSwitch += (_, args) =>
        {
            switch (args.Reason)
            {
                case SessionSwitchReason.SessionLock:
                    {
                        logger.LogInfo(type, "Stopping servers due to logout");

                        ids = app.Servers.Where(x => x.Status.Working).Select(x =>
                        {
                            x.Stop();
                            return x.Id;
                        }).ToList();

                        break;
                    }
                case SessionSwitchReason.SessionUnlock:
                    {
                        logger.LogInfo(type, "Restoring servers");

                        ids.ForEach(id => app.Servers.Single(s => s.Id == id).Start());
                        break;
                    }
                default:
                    break;
            }
        };

        try
        {
            app.Run();
        }
        catch (Exception e)
        {
            logger.LogError(type, e.Message);
        }
    }
}
