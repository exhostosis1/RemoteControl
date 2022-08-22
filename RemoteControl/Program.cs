using System.Runtime.InteropServices;

namespace RemoteControl
{
    public static class Program
    {
        public static async Task Main()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var container = new RemoteControlMain();

                var config = container.Config.GetConfig();

                container.Server.Start(config.UriConfig.Uri);
                container.Autostart.SetAutostart(config.Common.Autostart);

                await Task.Delay(-1);
            }
        }
    }
}
