using Shared;

namespace RemoteControlMain
{
    public static class Main
    {
        public static void Run(IContainer container)
        {
            var uri = container.ConfigProvider.Config.Uri;
            var ui = container.UserInterface;

            container.Server.Start(uri);

            ui.IsListening = container.Server.IsListening;
            ui.IsAutostart = container.AutostartService.CheckAutostart();
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
                container.AutostartService.SetAutostart(value);
                ui.IsAutostart = container.AutostartService.CheckAutostart();
            };

            ui.RunUI();
        }
    }
}
