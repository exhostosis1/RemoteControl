namespace RemoteControl
{
    public static class Program
    {
        public static void Main()
        {
            var container = new RemoteControlMain();

            var config = container.Config.GetConfig();

            container.Server.Start(config.UriConfig.Uri);
            container.Autostart.SetAutostart(config.Common.Autostart);

            Console.ReadLine();
        }
    }
}
