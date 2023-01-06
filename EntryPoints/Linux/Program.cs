using System.Runtime.InteropServices;

namespace Linux;

public static class Program
{
    public static void Main()
    {
        var container = new RemoteControlContainer();

        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            container.Logger.LogError("OS not supported");
            return;
        }

        if (Environment.UserName != "root")
        {
            container.Logger.LogError("Should run as root");
            return;
        }

        RemoteControl.Main.Run(container);
    }
}