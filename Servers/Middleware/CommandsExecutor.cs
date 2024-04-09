using Microsoft.Extensions.Logging;
using Servers.DataObjects;
using Servers.DataObjects.Bot;
using Shared.ControlProviders.Provider;
using Shared.Enums;

namespace Servers.Middleware;

public class CommandsExecutor(IKeyboardControlProvider keyboard, IDisplayControlProvider display, IAudioControlProvider audio, ILogger logger) : IMiddleware
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
                keyboard.KeyboardKeyPress(KeysEnum.MediaPlayPause);
                break;
            case BotButtons.MediaBack:
                keyboard.KeyboardKeyPress(KeysEnum.MediaPrev);
                break;
            case BotButtons.MediaForth:
                keyboard.KeyboardKeyPress(KeysEnum.MediaNext);
                break;
            case BotButtons.VolumeUp:
                var volume = audio.GetVolume();
                volume += 5;
                volume = volume > 100 ? 100 : volume;
                audio.SetVolume(volume);
                context.BotResponse.Message = volume.ToString();
                return Task.CompletedTask;
            case BotButtons.VolumeDown:
                volume = audio.GetVolume();
                volume -= 5;
                volume = volume < 0 ? 0 : volume > 100 ? 100 : volume;
                audio.SetVolume(volume);
                context.BotResponse.Message = volume.ToString();
                return Task.CompletedTask;
            case BotButtons.Darken:
                display.DisplayOff();
                break;
            default:
                if (int.TryParse(context.BotRequest.Command, out volume))
                {
                    volume = volume < 0 ? 0 : volume > 100 ? 100 : volume;
                    audio.SetVolume(volume);
                }
                break;
        }

        context.BotResponse.Message = "done";

        return Task.CompletedTask;
    }
}