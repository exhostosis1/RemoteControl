using System.Runtime.InteropServices;

namespace RemoteControlLinux
{
    public static class Program
    {
        public static void Main()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                throw new Exception("OS not supported");

            if (Environment.UserName != "root")
                throw new Exception("Should run as root");

            var container = new RemoteControlContainer();
            RemoteControlMain.Main.Run(container);
        }
    }
}
