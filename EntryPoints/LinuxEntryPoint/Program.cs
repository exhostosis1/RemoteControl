using AutoStart;
using ConfigProviders;
using ConsoleUI;
using ControlProviders.Wrappers;
using RemoteControlMain;
using Shared.DIContainer;
using System.Runtime.InteropServices;

namespace LinuxEntryPoint;

public static class Program
{
    public static void Main()
    {
        var app = new AppBuilder(new ContainerBuilder())
            .RegisterBasicDependencies()
            .RegisterConfig<LocalFileConfigProvider>()
            .RegisterAutoStart<DummyAutoStartService>()
            .RegisterUserInterface<MainConsole>()
            .RegisterInputs<YdoToolWrapper, YdoToolWrapper, DummyWrapper, DummyWrapper>()
            .Build();

        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            app.Logger.LogError(typeof(Program), "OS not supported");
            return;
        }

        if (Environment.UserName != "root")
        {
            app.Logger.LogError(typeof(Program), "Should run as root");
            return;
        }

        app.Run();
    }
}