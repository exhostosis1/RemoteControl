using Shared;

namespace RemoteControlMain;

public static class Main
{
    public static void Run(IContainer container)
    {
        var uri = container.ConfigProvider.ConfigUri;
        var ui = container.UserInterface;

        try
        {
            container.Server.Start(uri);
        }
        catch (Exception e)
        {
            container.Logger.LogError(e.Message);
        }

        ui.IsListening = container.Server.IsListening;
        ui.IsAutostart = container.AutostartService.CheckAutostart();
        ui.Uri = uri;

        ui.StartEvent += () =>
        {
            try
            {
                container.Server.Start(uri);
            }
            catch (Exception e)
            {
                container.Logger.LogError(e.Message);
            }
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

        ui.UriChangeEvent += value =>
        {
            ui.Uri = value;

            container.Server.Stop();
            try
            {
                container.Server.Start(value);
                container.ConfigProvider.ConfigUri = value;
            }
            catch (Exception e)
            {
                ui.Uri = null;
                container.Logger.LogError(e.Message);
                ui.ShowError(e.Message);
            }

            ui.IsListening = container.Server.IsListening;
            uri = value;
        };

        ui.CloseEvent += () => Environment.Exit(0);

        ui.RunUI();
    }
}