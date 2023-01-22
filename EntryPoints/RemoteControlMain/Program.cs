using Servers;
using Shared;
using Shared.Config;
using Shared.DataObjects.Bot;
using Shared.DataObjects.Http;
using Shared.Logging;
using Shared.Server;

namespace RemoteControlMain;

public static class Program
{
    private static int _id;

    private static IServer CreateSimpleServer(IContainer container, ServerConfig? config = null)
    {
        var result = new SimpleServer<HttpContext, ServerConfig>(container.NewWebListener(container.NewHttpListener(container.Logger), container.Logger), container.ApiMiddleware, new LogWrapper<SimpleServer<HttpContext, ServerConfig>>(container.Logger), config)
        {
            Id = _id++
        };

        return result;
    }

    private static IServer CreateTelegramBot(IContainer container, BotConfig? config = null)
    {
        var result = new SimpleServer<BotContext, BotConfig>(container.NewBotListener(container.NewTelegramBotApiProvider(container.NewHttpClient(), container.Logger), container.Logger), container.CommandExecutor, new LogWrapper<SimpleServer<BotContext, BotConfig>>(container.Logger), config)
        {
            Id = _id++
        };

        return result;
    }

    private static IEnumerable<IServer> CreateProcessors(AppConfig config, IContainer container)
    {
        return config.ProcessorConfigs.Select(x => x switch
        {
            ServerConfig s => CreateSimpleServer(container, s),
            BotConfig b => CreateTelegramBot(container, b),
            _ => throw new NotSupportedException("Config not supported")
        });
    }

    private static AppConfig GetConfig(IEnumerable<IServer> processors) =>
        new(processors.Select(x => x.Config));

    public static List<IServer> ControlProcessors { get; private set; } = new();

    public static void Run(IPlatformDependantContainer lesserContainer)
    {
        var container = new Container(lesserContainer);

        var ui = container.UserInterface;
        var config = container.ConfigProvider.GetConfig();

        ControlProcessors = CreateProcessors(config, container).ToList();

        ControlProcessors.ForEach(x =>
        {
            if (x.Config.Autostart)
                x.Start();
        });
        
        ui.SetAutostartValue(container.AutostartService.CheckAutostart());

        ui.StartEvent += (sender, id) =>
        {
            if(!id.HasValue)
            {
                ControlProcessors.ForEach(x => x.Start());
            }
            else
            {
                ControlProcessors.FirstOrDefault(x => x.Id == id)?.Start();
            }
        };

        ui.StopEvent += (sender, id) =>
        {
            if (!id.HasValue)
            {
                ControlProcessors.ForEach(x => x.Stop());
            }
            else
            {
                ControlProcessors.FirstOrDefault(x => x.Id == id)?.Stop();
            }
        };

        ui.ProcessorAddedEvent += (sender, mode) =>
        {
            var processor = mode switch
            {
                "server" => CreateSimpleServer(container),
                "bot" => CreateTelegramBot(container),
                _ => throw new NotSupportedException()
            };

            ControlProcessors.Add(processor);
            ui.AddProcessor(processor);
        };

        ui.ProcessorRemovedEvent += (sender, id) =>
        {
            var processor = ControlProcessors.FirstOrDefault(x => x.Id == id);
            if (processor == null)
                return;

            processor.Stop();
            ControlProcessors.Remove(processor);

            container.ConfigProvider.SetConfig(GetConfig(ControlProcessors));
        };

        ui.AutostartChangedEvent += (sender, value) =>
        {
            container.AutostartService.SetAutostart(value);
            ui.SetAutostartValue(container.AutostartService.CheckAutostart());
        };

        ui.ConfigChangedEvent += (_, configTuple) =>
        {
            var processor = ControlProcessors.FirstOrDefault(x => x.Id == configTuple.Item1);
            if (processor == null)
                return;

            if (processor.Status.Working)
            {
                processor.Restart(configTuple.Item2);
            }
            else
            {
                processor.Config = configTuple.Item2;
            }

            config = GetConfig(ControlProcessors);
            container.ConfigProvider.SetConfig(config);
        };

        ui.CloseEvent += (sender, args) =>
        {
            Environment.Exit(0);
        };

        ui.RunUI(ControlProcessors);
    }
}