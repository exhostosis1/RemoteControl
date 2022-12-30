using Bots;
using Controllers;
using Listeners;
using Servers;
using Servers.Endpoints;
using Servers.Middleware;
using Shared;
using Shared.Config;
using Shared.Controllers;
using Shared.Enums;
using Shared.Server;

namespace RemoteControlMain;

public static class Main
{
    public static IControlProcessor CreateServer(IContainer container, string name, ServerConfig config)
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
        var middleware = new RoutingMiddleware(new AbstractEndpoint[] { staticFilesEndpoint, apiEndpoint }, container.Logger);
        var server = new SimpleServer(name, listener, middleware, container.Logger, config);

        return server;
    }

    public static IControlProcessor CreateBot(IContainer container, string name, BotConfig config)
    {
        var executor = new CommandsExecutor(container.ControlProviders.Keyboard, container.Logger);
        var bot = new TelegramBot(name, executor, container.Logger, config);

        return bot;
    }

    public static IEnumerable<IControlProcessor> ControlProcessors { get; private set; } = Enumerable.Empty<IControlProcessor>();

    public static void Run(IContainer container)
    {
        var ui = container.UserInterface;
        var config = container.ConfigProvider.GetConfig();

        var servers = config.Servers.Select(x => CreateServer(container, x.Key, x.Value)).ToList();
        var bots = config.Bots.Select(x => CreateBot(container, x.Key, x.Value)).ToList();

        ControlProcessors = servers.Concat(bots).ToList();

        var dtos = new List<ControlProcessorDto>();

        foreach (var controlProcessor in ControlProcessors)
        {
            if(controlProcessor.CurrentConfig.Autostart)
                controlProcessor.Start();

            switch (controlProcessor.Type)
            {
                case ControlProcessorType.Server:
                    dtos.Add(new ServerDto
                    {
                        Name = controlProcessor.Name,
                        Running = controlProcessor.Status == ControlProcessorStatus.Working,
                        ListeningUri =
                            (controlProcessor.CurrentConfig as ServerConfig)?.Uri?.ToString() ?? string.Empty,
                    });
                    break;
                case ControlProcessorType.Bot:
                    dtos.Add(new BotDto
                    {
                        Name = controlProcessor.Name,
                        ApiKey = (controlProcessor.CurrentConfig as BotConfig)?.ApiKey ?? string.Empty,
                        ApiUrl = (controlProcessor.CurrentConfig as BotConfig)?.ApiUri ?? string.Empty,
                        BotUsernames = string.Join(';',
                            (controlProcessor.CurrentConfig as BotConfig)?.Usernames ?? Enumerable.Empty<string>()),
                        Running = controlProcessor.Status == ControlProcessorStatus.Working
                    });
                    break;
                default:
                    break;
            }
        }

        ui.SetViewModel(dtos);

        ui.StartEvent += (name, type) =>
        {
            throw new NotImplementedException();
        };

        ui.StopEvent += (name, type) =>
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
            throw new NotImplementedException();
        };

        ui.AddFirewallRuleEvent += () =>
        {
            foreach (var uri in servers)
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