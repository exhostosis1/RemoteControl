using Shared;
using Shared.Config;

namespace RemoteControlMain;

public static class Main
{
    public static void Run(IContainer container)
    {
        var config = container.ConfigProvider.GetConfig();
        var ui = container.UserInterface;

        try
        {
            container.Server.Start(config);
        }
        catch (Exception e)
        {
            container.Logger.LogError(e.Message);
        }

        ui.IsListening = container.Server.IsListening;
        ui.IsAutostart = container.AutostartService.CheckAutostart();
        ui.Uri = config.ServerConfig.Uri;

        ui.StartEvent += () =>
        {
            try
            {
                container.Server.Start(config);
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
                config.ServerConfig.Scheme = value.Scheme;
                config.ServerConfig.Host = value.Host;
                config.ServerConfig.Port = value.Port;

                container.Server.Start(config);

                container.ConfigProvider.SetConfig(config);
            }
            catch (Exception e)
            {
                ui.Uri = null;
                container.Logger.LogError(e.Message);
                ui.ShowError(e.Message);
            }

            ui.IsListening = container.Server.IsListening;
        };

        ui.CloseEvent += () => Environment.Exit(0);

        ui.RunUI();
    }
}