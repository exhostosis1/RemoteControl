using System.Runtime.InteropServices;

namespace LinuxEntryPoint;

public static class Program
{
    public static void Main()
    {
        var container = new RemoteControlContainer();
        var logger = container.Logger;

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