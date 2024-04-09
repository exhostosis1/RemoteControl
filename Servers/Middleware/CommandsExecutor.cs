using Microsoft.Extensions.Logging;
using Shared;
using Shared.ControlProviders.Provider;
using Shared.DataObjects;
using Shared.DataObjects.Bot;
using Shared.Enums;

namespace Servers.Middleware;

public class CommandsExecutor(IGeneralControlProvider controlFacade, ILogger logger) : IMiddleware
{
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

    public Task ProcessRequestAsync(IContext contextParam, RequestDelegate _)
    {
        var context = (BotContext)contextParam;

        logger.LogInformation("Executing bot command {command}", context.BotRequest.Command);

        context.BotResponse.Buttons = _buttons;

        switch (context.BotRequest.Command)
        {
            case BotButtons.Pause:
                controlFacade.KeyboardKeyPress(KeysEnum.MediaPlayPause);
                break;
            case BotButtons.MediaBack:
                controlFacade.KeyboardKeyPress(KeysEnum.MediaPrev);
                break;
            case BotButtons.MediaForth:
                controlFacade.KeyboardKeyPress(KeysEnum.MediaNext);
                break;
            case BotButtons.VolumeUp:
                var volume = controlFacade.GetVolume();
                volume += 5;
                volume = volume > 100 ? 100 : volume;
                controlFacade.SetVolume(volume);
                context.BotResponse.Message = volume.ToString();
                return Task.CompletedTask;
            case BotButtons.VolumeDown:
                volume = controlFacade.GetVolume();
                volume -= 5;
                volume = volume < 0 ? 0 : volume > 100 ? 100 : volume;
                controlFacade.SetVolume(volume);
                context.BotResponse.Message = volume.ToString();
                return Task.CompletedTask;
            case BotButtons.Darken:
                controlFacade.DisplayOff();
                break;
            default:
                if (int.TryParse(context.BotRequest.Command, out volume))
                {
                    volume = volume < 0 ? 0 : volume > 100 ? 100 : volume;
                    controlFacade.SetVolume(volume);
                }
                break;
        }

        context.BotResponse.Message = "done";

        return Task.CompletedTask;
    }
}