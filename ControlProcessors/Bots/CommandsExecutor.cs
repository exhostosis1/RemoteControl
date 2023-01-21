using Shared.ControlProviders;
using Shared.DataObjects.Bot;
using Shared.Enums;
using Shared.Logging.Interfaces;
using Shared.Server;

namespace Bots;

public class CommandsExecutor: AbstractMiddleware<BotContext>
{
    private readonly ILogger<CommandsExecutor> _logger;

    private readonly IControlProvider _controlFacade;

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

    public CommandsExecutor(IControlProvider controlFacade, ILogger<CommandsExecutor> logger)
    {
        _logger = logger;
        _controlFacade = controlFacade;
    }

    public override void ProcessRequest(BotContext context)
    {
        _logger.LogInfo($"Executing bot command {context.Request.Command}");

        var volume = _controlFacade.GetVolume();
        
        context.Response.Buttons = _buttons;

        switch (context.Request.Command)
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
                volume += 5;
                volume = volume > 100 ? 100 : volume;
                _controlFacade.SetVolume(volume);
                context.Response.Message = volume.ToString();
                return;
            case BotButtons.VolumeDown:
                volume -= 5;
                volume = volume < 0 ? 0 : volume;
                _controlFacade.SetVolume(volume);
                context.Response.Message = volume.ToString();
                return;
            case BotButtons.Darken:
                _controlFacade.DisplayOff();
                break;
            default:
                if (int.TryParse(context.Request.Command, out volume))
                {
                    volume = volume < 0 ? 0 : volume > 100 ? 100 : volume;
                    _controlFacade.SetVolume(volume);
                }
                break;
        }

        context.Response.Message = "done";
    }
}