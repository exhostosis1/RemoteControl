using Shared.ApiControllers;
using Shared.Bot;
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
            case Buttons.Pause:
                _controlFacade.Keyboard.KeyPress(KeysEnum.MediaPlayPause);
                break;
            case Buttons.MediaBack:
                _controlFacade.Keyboard.KeyPress(KeysEnum.MediaPrev);
                break;
            case Buttons.MediaForth:
                _controlFacade.Keyboard.KeyPress(KeysEnum.MediaNext);
                break;
            case Buttons.VolumeUp:
                volume += 5;
                volume = volume > 100 ? 100 : volume;
                _controlFacade.Audio.SetVolume(volume);
                return volume.ToString();
            case Buttons.VolumeDown:
                volume -= 5;
                volume = volume < 0 ? 0 : volume;
                _controlFacade.Audio.SetVolume(volume);
                return volume.ToString();
            case Buttons.Darken:
                _controlFacade.Display.Darken();
                break;
            default:
                if (int.TryParse(command, out volume))
                {
                    volume = volume < 0 ? 0 : volume > 100 ? 100 : volume;
                    _controlFacade.Audio.SetVolume(volume);
                }
                return volume.ToString();
        }

        return "done";
    }
}