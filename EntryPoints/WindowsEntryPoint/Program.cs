using AutoStart;
using ConfigProviders;
using ControlProviders.Wrappers;
using Microsoft.Win32;
using RemoteControlMain;
using Shared.DIContainer;
using Shared.Enums;
using Shared.Wrappers.Registry;
using Shared.Wrappers.RegistryWrapper;
using System.Runtime.InteropServices;
using WinFormsUI;

namespace WindowsEntryPoint;

public static class Program
{
    public static void Main()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return;

        var app = new AppBuilder(new ContainerBuilder())
            .RegisterBasicDependencies()
            .RegisterConfig<LocalFileConfigProvider>()
            .RegisterUserInterface<MainForm>()
            .RegisterInputs<User32Wrapper, User32Wrapper, User32Wrapper, NAudioWrapper>()
            .RegisterAutoStart<RegistryAutoStartService>()
            .RegisterDependency<IRegistry, RegistryWrapper>(Lifetime.Singleton)
            .Build();
        
        var type = typeof(Program);

        var ids = new List<int>();

        SystemEvents.SessionSwitch += (_, args) =>
        {
            switch (args.Reason)
            {
                case SessionSwitchReason.SessionLock:
                    {
                        app.Logger.LogInfo(type, "Stopping servers due to logout");

                        ids = app.Servers.Where(x => x.Status.Working).Select(x =>
                        {
                            x.Stop();
                            return x.Id;
                        }).ToList();

                        break;
                    }
                case SessionSwitchReason.SessionUnlock:
                    {
                        app.Logger.LogInfo(type, "Restoring servers");

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
            app.Logger.LogError(type, e.Message);
        }
    }
}
