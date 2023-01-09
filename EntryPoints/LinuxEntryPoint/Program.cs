using System.Runtime.InteropServices;

namespace LinuxEntryPoint;

public static class Program
{
    public static void Main()
    {
        var container = new RemoteControlContainer();
        var logger = container.GetLogger(typeof(Program));

        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            logger.LogError("OS not supported");
            return;
        }

        if (Environment.UserName != "root")
        {
            logger.LogError("Should run as root");
            return;
        }

        RemoteControlMain.Program.Run(container);
    }
}