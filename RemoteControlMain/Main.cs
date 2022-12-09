using Shared;

namespace RemoteControlMain;

public static class Main
{
    public static void Run(IContainer container)
    {
        var ui = container.UserInterface;
        var config = container.ConfigProvider.GetConfig();

        foreach (var processor in container.ControlProcessors)
        {
            var c = config.GetProcessorConfigByName(processor.Name);

            if (c?.Autostart ?? false)
            {
                processor.Start(c);
            }
        }

        ui.StartEvent += name =>
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                var c = config.GetProcessorConfigByName(name);

                container.ControlProcessors.FirstOrDefault(x => x.Name == name)?.Start(c);

                c.Autostart = true;
            }
            else
            {
                foreach (var processor in container.ControlProcessors)
                {
                    processor.Start(config.ProcessorConfigs.FirstOrDefault(x => x.Name == processor.Name));
                }
            }
        };

        ui.StopEvent += name =>
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                var c = config.GetProcessorConfigByName(name);

                container.ControlProcessors.FirstOrDefault(x => x.Name == name)?.Stop();

                c.Autostart = false;
            }
            else
            {
                foreach (var processor in container.ControlProcessors)
                {
                    processor.Stop();
                }
            }
        };

        ui.AutostartChangedEvent += value =>
        {
            container.AutostartService.SetAutostart(value);
            ui.IsAutostart = container.AutostartService.CheckAutostart();
        };

        ui.ConfigChangedEvent += value =>
        {
            var c = config.GetProcessorConfigByName(value.Name);
            if (c != null)
                c.Host = value.Info;
        };

        ui.AddFirewallRuleEvent += () =>
        {
            foreach (var uri in config.ProcessorConfigs.Where(x => x.Uri != null))
            {
                var command =
                    $"netsh advfirewall firewall add rule name=\"Remote Control\" dir=in action=allow profile=private localip={uri.Host} localport={uri.Port} protocol=tcp";

                Utils.RunWindowsCommandAsAdmin(command);
            }
        };

        ui.CloseEvent += () =>
        {
            Environment.Exit(0);
        };

        ui.RunUI(config);
    }
}