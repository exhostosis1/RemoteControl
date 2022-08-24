using System.Runtime.InteropServices;

namespace RemoteControl
{
    public static class Program
    {
        public static void Main()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && Environment.UserName != "root")
            {
                Console.WriteLine("Should run as root");
                return;
            }

            var container = new RemoteControlMain();

            var config = container.Config.GetConfig();

            container.Server.Start(config.UriConfig.Uri);
            container.Autostart.SetAutostart(config.Common.Autostart);

            Console.ReadLine();
        }
    }
}
