using Shared.Config;
using Shared.ControlProcessor;
using Shared.Listeners;
using Shared.Logging.Interfaces;

namespace Bots;

public class TelegramBot: BotProcessor
{
    private readonly IBotListener _listener;
    
    public TelegramBot(IBotListener listener, ILogger logger, BotConfig? config = null) : base(logger, config)
    {
        _listener = listener;
    }

    protected override void StartInternal(BotConfig config)
    {
        if (Working) Stop();

        if(string.IsNullOrWhiteSpace(CurrentConfig.ApiUri) || string.IsNullOrWhiteSpace(CurrentConfig.ApiKey))
        {
            Logger.LogError("Wrong bot api configuration");
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