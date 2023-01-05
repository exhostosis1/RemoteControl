using Bots;
using Servers;
using Shared;
using Shared.Config;
using Shared.ControlProcessor;

namespace RemoteControlMain;

public static class Main
{
    private static IControlProcessor CreateSimpleServer(IContainer container, ServerConfig? config = null) =>
        new SimpleServer(container.Listener, container.Middleware, container.Logger, config);

    private static IControlProcessor CreateTelegramBot(IContainer container, BotConfig? config = null) =>
        new TelegramBot(container.CommandExecutor, container.Logger, config);

    private static IEnumerable<IControlProcessor> CreateProcessors(AppConfig config, IContainer container) =>
        config.All.Select(x =>
            x switch
            {
                ServerConfig s => CreateSimpleServer(container, s),
                BotConfig b => CreateTelegramBot(container, b),
                _ => throw new NotSupportedException()
            }
        );

    private static AppConfig GetConfig(IEnumerable<IControlProcessor> processors)
    {
        var result = new AppConfig();

        foreach (var controlProcessor in processors)
        {
            switch (controlProcessor)
            {
                case IServerProcessor s:
                    result.Servers.Add(s.CurrentConfig);
                    break;
                case IBotProcessor b:
                    result.Bots.Add(b.CurrentConfig);
                    break;
            }
        }

        return result;
    }

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

        ui.SetViewModel(ControlProcessors);

        ui.StartEvent += index =>
        {
            if(!index.HasValue)
            {
                ControlProcessors.ForEach(x => x.Start());
            }
            else if (index.Value < ControlProcessors.Count)
            {
                ControlProcessors[index.Value].Start();
            }
        };

        ui.StopEvent += index =>
        {
            if (!index.HasValue)
            {
                ControlProcessors.ForEach(x => x.Stop());
            }
            else if (index.Value < ControlProcessors.Count)
            {
                ControlProcessors[index.Value].Stop();
            }
        };

        ui.ProcessorAddedEvent += mode =>
        {
            switch (mode)
            {
                case "server":
                    ControlProcessors.Add(CreateSimpleServer(container));
                    break;
                case "bot":
                    ControlProcessors.Add(CreateTelegramBot(container));
                    break;
                default:
                    return;
            }

            ui.SetViewModel(ControlProcessors);
        };

        ui.AutostartChangedEvent += value =>
        {
            container.AutostartService.SetAutostart(value);
            ui.SetAutostartValue(container.AutostartService.CheckAutostart());
        };

        ui.ConfigChangedEvent += (index, c) =>
        {
            if (ControlProcessors[index].Working)
            {
                ControlProcessors[index].Restart(c);
            }
            else
            {
                ControlProcessors[index].CurrentConfig = c;
            }

            config = GetConfig(ControlProcessors);
            container.ConfigProvider.SetConfig(config);

            ui.SetViewModel(ControlProcessors);
        };

        ui.CloseEvent += () =>
        {
            Environment.Exit(0);
        };

        ui.RunUI();
    }
}