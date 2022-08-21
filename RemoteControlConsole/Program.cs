using RemoteControl;

namespace RemoteControlConsole
{
    internal static class Program
    {
        private static void Main()
        {
            var config = RemoteControlMain.Config.GetConfig();
            
            RemoteControlMain.AutostartService.SetAutostart(config.Common.Autostart);
            RemoteControlMain.Server.Start(config.UriConfig.Uri);

            Console.ReadLine();
        }
    }
}