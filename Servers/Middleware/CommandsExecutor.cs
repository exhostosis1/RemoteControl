using Shared.ControlProviders.Provider;
using Shared.DataObjects.Bot;
using Shared.Enums;
using Shared.Logging.Interfaces;
using Shared.Server;

namespace Servers.Middleware;

public class CommandsExecutor : IBotMiddleware
{
    private readonly ILogger<CommandsExecutor> _logger;

    private readonly IGeneralControlProvider _controlFacade;

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

    public CommandsExecutor(IGeneralControlProvider controlFacade, ILogger<CommandsExecutor> logger)
    {
        _logger = logger;
        _controlFacade = controlFacade;
    }

    public event EventHandler<BotContext>? OnNext;

    public void ProcessRequest(object? _, BotContext context)
    {
        _logger.LogInfo($"Executing bot command {context.BotRequest.Command}");

        context.BotResponse.Buttons = _buttons;

        switch (context.BotRequest.Command)
        {
            case BotButtons.Pause:
                _controlFacade.KeyboardKeyPress(KeysEnum.MediaPlayPause);
                break;
            case BotButtons.MediaBack:
                _controlFacade.KeyboardKeyPress(KeysEnum.MediaPrev);
                break;
            case BotButtons.MediaForth:
                _controlFacade.KeyboardKeyPress(KeysEnum.MediaNext);
                break;
            case BotButtons.VolumeUp:
                var volume = _controlFacade.GetVolume();
                volume += 5;
                volume = volume > 100 ? 100 : volume;
                _controlFacade.SetVolume(volume);
                context.BotResponse.Message = volume.ToString();
                return;
            case BotButtons.VolumeDown:
                volume = _controlFacade.GetVolume();
                volume -= 5;
                volume = volume < 0 ? 0 : volume > 100 ? 100 : volume;
                _controlFacade.SetVolume(volume);
                context.BotResponse.Message = volume.ToString();
                return;
            case BotButtons.Darken:
                _controlFacade.DisplayOff();
                break;
            default:
                if (int.TryParse(context.BotRequest.Command, out volume))
                {
                    volume = volume < 0 ? 0 : volume > 100 ? 100 : volume;
                    _controlFacade.SetVolume(volume);
                }
                break;
        }

        context.BotResponse.Message = "done";
    }
}