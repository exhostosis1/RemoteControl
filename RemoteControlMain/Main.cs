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
using System.Reflection;

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

    public static List<IControlProcessor> CreateProcessors(AppConfig config, IContainer container)
    {
        return config.Servers.Select(x => CreateServer(container, x.Key, x.Value))
            .Concat(config.Bots.Select(x => CreateBot(container, x.Key, x.Value))).ToList();
    }

    public static List<ControlProcessorDto> GetDtos()
    {
        var dtos = new List<ControlProcessorDto>();

        foreach (var controlProcessor in ControlProcessors)
        {
            if (controlProcessor.CurrentConfig.Autostart)
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

        return dtos;
    }

    public static IEnumerable<IControlProcessor> ControlProcessors { get; private set; } = Enumerable.Empty<IControlProcessor>();

    public static void Run(IContainer container)
    {
        var ui = container.UserInterface;
        var config = container.ConfigProvider.GetConfig();

        ControlProcessors = CreateProcessors(config, container);

        ui.SetViewModel(GetDtos());

        void StartOrStop(bool start, string? name, ControlProcessorType type)
        {
            var methodName = start ? "Start" : "Stop";

            if (!string.IsNullOrEmpty(name))
            {
                var item = ControlProcessors.FirstOrDefault(x => x.Name == name && x.Type == type);
                if (item == null)
                    return;

                var method = item?.GetType()
                    .GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance);

                method?.Invoke(item, Array.Empty<object>());

                if (type == ControlProcessorType.Bot)
                {
                    config.Bots[name].Autostart = true;
                }
                else if (type == ControlProcessorType.Server)
                {
                    config.Servers[name].Autostart = true;
                }
            }
            else
            {
                foreach (var controlProcessor in ControlProcessors)
                {
                    var method = controlProcessor.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance);
                    method?.Invoke(controlProcessor, Array.Empty<object>());
                }

                foreach (var configServer in config.Servers)
                {
                    configServer.Value.Autostart = true;
                }

                foreach (var configBot in config.Bots)
                {
                    configBot.Value.Autostart = true;
                }
            }

            container.ConfigProvider.SetConfig(config);

            ui.SetViewModel(GetDtos());
        }

        ui.StartEvent += (name, type) => StartOrStop(true, name, type);

        ui.StopEvent += (name, type) => StartOrStop(false, name, type);

        ui.AutostartChangedEvent += value =>
        {
            container.AutostartService.SetAutostart(value);
            ui.SetAutostartValue(container.AutostartService.CheckAutostart());
        };

        ui.ConfigChangedEvent += value =>
        {
            config.Servers.Clear();
            config.Bots.Clear();

            foreach (var controlProcessor in ControlProcessors)
            {
                controlProcessor.Stop();
            }

            foreach (var controlProcessorDto in value)
            {
                switch (controlProcessorDto)
                {
                    case ServerDto s:
                        config.Servers.Add(s.Name, new ServerConfig
                        {
                            Uri = new Uri(s.ListeningUri),
                            Autostart = true
                        }); 
                        break;
                    case BotDto b:
                        config.Bots.Add(b.Name, new BotConfig
                        {
                            ApiKey = b.ApiKey,
                            ApiUri = b.ApiUrl,
                            Usernames = b.BotUsernames.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList()
                        });
                        break;
                    default:
                        break;
                }
            }

            container.ConfigProvider.SetConfig(config);

            ControlProcessors = CreateProcessors(config, container);

            foreach (var controlProcessor in ControlProcessors)
            {
                controlProcessor.Start();
            }

            ui.SetViewModel(GetDtos());
        };

        ui.AddFirewallRuleEvent += () =>
        {
            foreach (var uri in ControlProcessors.Where(x => x.Type == ControlProcessorType.Server))
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