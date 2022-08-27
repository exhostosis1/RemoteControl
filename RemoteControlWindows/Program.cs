using System.Runtime.InteropServices;

namespace RemoteControlWindows
{
    public static class Program
    {
        public static void Main()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                throw new Exception("OS not supported");

            var container = new RemoteControlContainer();
            RemoteControlMain.Main.Run(container);
        }
    }
}
