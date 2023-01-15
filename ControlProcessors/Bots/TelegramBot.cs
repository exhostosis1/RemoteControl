using Shared.ApiControllers;
using Shared.Config;
using Shared.ControlProcessor;
using Shared.DataObjects.Bot;
using Shared.Enums;
using Shared.Listeners;
using Shared.Logging.Interfaces;

namespace Bots;

public class TelegramBot: BotProcessor
{
    private readonly IBotListener _listener;
    private readonly ILogger<TelegramBot> _logger;

    private readonly ButtonsMarkup _buttons = new ReplyButtonsMarkup(new List<List<SingleButton>>
    {
        new()
        {
            new SingleButton(BotButtons.MediaBack),
            new SingleButton(BotButtons.Pause),
            new SingleButton(BotButtons.MediaForth)
        },
        new()
        {
            new SingleButton(BotButtons.VolumeDown),
            new SingleButton(BotButtons.Darken),
            new SingleButton(BotButtons.VolumeUp)
        }
    })
    {
        Resize = true,
        Persistent = true
    };

    public TelegramBot(IBotListener listener, ICommandExecutor executor, ILogger<TelegramBot> logger, BotConfig? config = null) : base(config)
    {
        _listener = listener;
        _logger = logger;

        _listener.OnStatusChange += (sender, status) =>
        {
            Working = status;
            StatusObservers.ForEach(x => x.OnNext(status));
        };
        _listener.OnRequest += (sender, context) =>
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

    public override void Stop()
    {
        if (!_listener.IsListening) return;

        _listener.StopListen();
    }
}