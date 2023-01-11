using Bots;
using Servers;
using Shared;
using Shared.Config;
using Shared.ControlProcessor;
using Shared.Logging;

namespace RemoteControlMain;

public static class Program
{
    private static int _id;

    private static AbstractControlProcessor CreateSimpleServer(IContainer container, ServerConfig? config = null)
    {
        var result = new SimpleServer(container.HttpListener, container.Middleware, new LogWrapper<SimpleServer>(container.Logger), config)
        {
            Id = _id++
        };

        return result;
    }

    private static AbstractControlProcessor CreateTelegramBot(IContainer container, BotConfig? config = null)
    {
        var result = new TelegramBot(container.BotListener, container.Executor, new LogWrapper<TelegramBot>(container.Logger), config)
        {
            Id = _id++
        };

        return result;
    }

    private static IEnumerable<AbstractControlProcessor> CreateProcessors(AppConfig config, IContainer container)
    {
        foreach (var configServer in config.Servers)
        {
            yield return CreateSimpleServer(container, configServer);
        }

        foreach (var configBot in config.Bots)
        {
            yield return CreateTelegramBot(container, configBot);
        }
    }

    private static AppConfig GetConfig(IEnumerable<AbstractControlProcessor> processors) =>
        new(processors.Select(x => x.Config));

    public static List<AbstractControlProcessor> ControlProcessors { get; private set; } = new();

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

        ui.StartEvent += id =>
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

        ui.StopEvent += id =>
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

        ui.ProcessorAddedEvent += mode =>
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

        ui.ProcessorRemovedEvent += id =>
        {
            var processor = ControlProcessors.FirstOrDefault(x => x.Id == id);
            if (processor == null)
                return;

            processor.Stop();
            ControlProcessors.Remove(processor);

            container.ConfigProvider.SetConfig(GetConfig(ControlProcessors));
        };

        ui.AutostartChangedEvent += value =>
        {
            container.AutostartService.SetAutostart(value);
            ui.SetAutostartValue(container.AutostartService.CheckAutostart());
        };

        ui.ConfigChangedEvent += (id, c) =>
        {
            var processor = ControlProcessors.FirstOrDefault(x => x.Id == id);
            if (processor == null)
                return;

            if (processor.Working)
            {
                processor.Restart(c);
            }
            else
            {
                processor.Config = c;
            }

            config = GetConfig(ControlProcessors);
            container.ConfigProvider.SetConfig(config);
        };

        ui.CloseEvent += () =>
        {
            Environment.Exit(0);
        };

        ui.RunUI(ControlProcessors);
    }
}