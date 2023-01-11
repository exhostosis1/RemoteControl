using Shared.ApiControllers;
using Shared.Bot;
using Shared.Config;
using Shared.ControlProcessor;
using Shared.Listeners;
using Shared.Logging.Interfaces;

namespace Bots;

public class TelegramBot: BotProcessor
{
    private readonly IBotListener _listener;
    private readonly ILogger<TelegramBot> _logger;

    private readonly string[][] _buttons = {
        new[] { Buttons.MediaBack, Buttons.Pause, Buttons.MediaForth },
        new[] { Buttons.VolumeDown, Buttons.Darken, Buttons.VolumeUp }
    };

    public TelegramBot(IBotListener listener, ICommandExecutor executor, ILogger<TelegramBot> logger, BotConfig? config = null) : base(config)
    {
        _listener = listener;
        _logger = logger;

        _listener.OnStatusChange += status => StatusObservers.ForEach(x => x.OnNext(status));
        _listener.OnRequest += context =>
        {
            context.Result = executor.Execute(context.Message);
            context.Buttons = _buttons;
        };
    }

    protected override void StartInternal(BotConfig config)
    {
        if (Working) Stop();

        if(string.IsNullOrWhiteSpace(CurrentConfig.ApiUri) || string.IsNullOrWhiteSpace(CurrentConfig.ApiKey))
        {
            _logger.LogError("Wrong bot api configuration");
            return;
        }

        _listener.StartListen(CurrentConfig.ApiUri, CurrentConfig.ApiKey, config.Usernames);
    }

    protected override void RestartInternal(BotConfig config)
    {
        Stop();
        Start(config);
    }

    public override void Stop()
    {
        if (!_listener.IsListening) return;

        _listener.StopListen();
    }
}