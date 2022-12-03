using Shared;

namespace RemoteControlMain;

public static class Main
{
    public static void Run(IContainer container)
    {
        var config = container.ConfigProvider.GetConfig();
        var ui = container.UserInterface;

        try
        {
            foreach (var containerControlProcessor in container.ControlProcessors)
            {
                containerControlProcessor.Start(config);  
            }
        }
        catch (Exception e)
        {
            container.Logger.LogError(e.Message);
        }
        
        ui.IsAutostart = container.AutostartService.CheckAutostart();

        ui.StartEvent += () =>
        {
            try
            {
                foreach (var containerControlProcessor in container.ControlProcessors)
                {
                    containerControlProcessor.Start(config);
                }
            }
            catch (Exception e)
            {
                container.Logger.LogError(e.Message);
            }
        };

        ui.StopEvent += () =>
        {
            foreach (var containerControlProcessor in container.ControlProcessors)
            {
                containerControlProcessor.Stop();
            }
        };

        ui.AutostartChangeEvent += value =>
        {
            container.AutostartService.SetAutostart(value);
            ui.IsAutostart = container.AutostartService.CheckAutostart();
        };

        ui.CloseEvent += () => Environment.Exit(0);

        ui.RunUI();
    }
}