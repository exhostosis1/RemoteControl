using System.Runtime.InteropServices;

namespace RemoteControl
{
    public static class Program
    {
        public static void Main()
        {
            var container = new RemoteControlContainer();

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && Environment.UserName != "root")
            {
                container.DefaultLogger.LogError("Should run as root");
                return;
            }

            var uri = container.Config.GetConfig().UriConfig.Uri;
            var ui = container.UserInterface;

            container.Server.Start(uri);

            ui.IsListening = container.Server.IsListening;
            ui.IsAutostart = container.Autostart.CheckAutostart();
            ui.Uri = uri;

            ui.StartEvent += () =>
            {
                container.Server.Start(uri);
                ui.IsListening = container.Server.IsListening;
            };
            ui.StopEvent += () =>
            {
                container.Server.Stop();
                ui.IsListening = container.Server.IsListening;
            };
            ui.AutostartChangeEvent += value =>
            {
                container.Autostart.SetAutostart(value);
                ui.IsAutostart = container.Autostart.CheckAutostart();
            };

            ui.RunUI();
        }
    }
}
