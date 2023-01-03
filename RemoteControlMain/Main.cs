using Bots;
using Controllers;
using Listeners;
using Servers;
using Servers.Endpoints;
using Servers.Middleware;
using Shared;
using Shared.Config;
using Shared.Controllers;
using Shared.ControlProcessor;

namespace RemoteControlMain;

public static class Main
{
    private static IControlProcessor CreateSimpleServer(IContainer container, ServerConfig config)
    {
        var controllers = new BaseController[]
        {
            new AudioController(container.ControlProviders.Audio, container.Logger),
            new DisplayController(container.ControlProviders.Display, container.Logger),
            new KeyboardController(container.ControlProviders.Keyboard, container.Logger),
            new MouseController(container.ControlProviders.Mouse, container.Logger)
        };

        var staticFilesEndpoint = new StaticFilesEndpoint(container.Logger);
        var apiEndpoint = new ApiV1Endpoint(controllers, container.Logger);

        var listener = new GenericListener(container.Logger);
        var middleware = new RoutingMiddleware(new []{ apiEndpoint }, staticFilesEndpoint, container.Logger);
        var server = new SimpleServer(config.Name, listener, middleware, container.Logger, config);

        return server;
    }

    private static IControlProcessor CreateTelegramBot(IContainer container, BotConfig config)
    {
        var executor = new CommandsExecutor(container.ControlProviders, container.Logger);
        var bot = new TelegramBot(config.Name, executor, container.Logger, config);

        return bot;
    }

    private static IEnumerable<IControlProcessor> CreateProcessors(AppConfig config, IContainer container)
    {
        return config.Servers.Select(x => CreateSimpleServer(container, x))
            .Concat(config.Bots.Select(x => CreateTelegramBot(container, x)));
    }

    public static List<IControlProcessor> ControlProcessors { get; private set; } = new();

    public static void Run(IContainer container)
    {
        var ui = container.UserInterface;
        var config = container.ConfigProvider.GetConfig();

        ControlProcessors = CreateProcessors(config, container).ToList();

        foreach (var controlProcessor in ControlProcessors.Where(x => x.CurrentConfig.Autostart))
        {
            controlProcessor.Start();
        }

        ui.SetViewModel(ControlProcessors);

        ui.StartEvent += index =>
        {
            if (index.HasValue && index.Value < ControlProcessors.Count)
            {
                ControlProcessors[index.Value].Start();
            }
            else if(!index.HasValue)
            {
                ControlProcessors.ForEach(x => x.Start());
            }
        };

        ui.StopEvent += index =>
        {
            if (index.HasValue && index.Value < ControlProcessors.Count)
            {
                ControlProcessors[index.Value].Stop();
            }
            else if (!index.HasValue)
            {
                ControlProcessors.ForEach(x => x.Stop());
            }
        };

        ui.ProcessorAddedEvent += _ =>
        {
            throw new NotImplementedException();
        };

        ui.AutostartChangedEvent += value =>
        {
            container.AutostartService.SetAutostart(value);
            ui.SetAutostartValue(container.AutostartService.CheckAutostart());
        };

        ui.ConfigChangedEvent += value =>
        {
            config.Servers.Clear();
            config.Bots.Clear();

            

            ui.SetViewModel(ControlProcessors);
        };

        ui.AddFirewallRuleEvent += () =>
        {
            foreach (var uri in ControlProcessors.Where(x => x is IServerProcessor))
            {
                var command =
                    $"netsh advfirewall firewall add rule name=\"Remote Control\" dir=in action=allow profile=private localip={(uri.CurrentConfig as ServerConfig)?.Host} localport={(uri.CurrentConfig as ServerConfig)?.Port} protocol=tcp";

                Utils.RunWindowsCommandAsAdmin(command);
            }
        };

        ui.CloseEvent += () =>
        {
            Environment.Exit(0);
        };

        ui.RunUI();
    }
}