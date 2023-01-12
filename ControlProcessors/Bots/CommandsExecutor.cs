using Shared.ApiControllers;
using Shared.ControlProviders;
using Shared.Enums;
using Shared.Logging.Interfaces;

namespace Bots;

public class CommandsExecutor: ICommandExecutor
{
    private readonly ILogger<CommandsExecutor> _logger;

    private readonly ControlFacade _controlFacade;

    public CommandsExecutor(ControlFacade controlFacade, ILogger<CommandsExecutor> logger)
    {
        _logger = logger;
        _controlFacade = controlFacade;
    }

    public string Execute(string command)
    {
        _logger.LogInfo($"Executing bot command {command}");

        var volume = _controlFacade.Audio.GetVolume();

        switch (command)
        {
            case BotButtons.Pause:
                _controlFacade.Keyboard.KeyPress(KeysEnum.MediaPlayPause);
                break;
            case BotButtons.MediaBack:
                _controlFacade.Keyboard.KeyPress(KeysEnum.MediaPrev);
                break;
            case BotButtons.MediaForth:
                _controlFacade.Keyboard.KeyPress(KeysEnum.MediaNext);
                break;
            case BotButtons.VolumeUp:
                volume += 5;
                volume = volume > 100 ? 100 : volume;
                _controlFacade.Audio.SetVolume(volume);
                return volume.ToString();
            case BotButtons.VolumeDown:
                volume -= 5;
                volume = volume < 0 ? 0 : volume;
                _controlFacade.Audio.SetVolume(volume);
                return volume.ToString();
            case BotButtons.Darken:
                _controlFacade.Display.Darken();
                break;
            default:
                if (int.TryParse(command, out volume))
                {
                    volume = volume < 0 ? 0 : volume > 100 ? 100 : volume;
                    _controlFacade.Audio.SetVolume(volume);
                }
                return "done";
        }

        return "done";
    }
}