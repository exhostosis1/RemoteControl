using Bots;
using Servers;
using Shared;
using Shared.Config;
using Shared.ControlProcessor;

namespace RemoteControlMain;

public static class Program
{
    private static int Id;

    private static IControlProcessor CreateSimpleServer(IContainer container, ServerConfig? config = null)
    {
        var result = new SimpleServer(container.Listener, container.Middleware, container.Logger, config)
        {
            Id = Id++
        };

        return result;
    }

    private static IControlProcessor CreateTelegramBot(IContainer container, BotConfig? config = null)
    {
        var result = new TelegramBot(container.CommandExecutor, container.Logger, config)
        {
            Id = Id++
        };

        return result;
    }

    private static IEnumerable<IControlProcessor> CreateProcessors(AppConfig config, IContainer container) =>
        config.Items.Select(x =>
            x switch
            {
                ServerConfig s => CreateSimpleServer(container, s),
                BotConfig b => CreateTelegramBot(container, b),
                _ => throw new NotSupportedException()
            }
        );

    private static AppConfig GetConfig(IEnumerable<IControlProcessor> processors) =>
        new(processors.Select(x => x.CurrentConfig));

    public static List<IControlProcessor> ControlProcessors { get; private set; } = new();

    public static void Run(IPlatformDependantContainer lesserContainer)
    {
        var container = new Container(lesserContainer);

        var ui = container.UserInterface;
        var config = container.ConfigProvider.GetConfig();

        ControlProcessors = CreateProcessors(config, container).ToList();

        ControlProcessors.ForEach(x =>
        {
            if (x.CurrentConfig.Autostart)
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
                processor.CurrentConfig = c;
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