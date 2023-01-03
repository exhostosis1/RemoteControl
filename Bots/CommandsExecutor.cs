using Shared.ControlProviders;
using Shared.Enums;
using Shared.Logging.Interfaces;

namespace Bots;

public class CommandsExecutor
{
    private readonly ILogger _logger;

    private readonly ControlFacade _controlFacade;

    public CommandsExecutor(ControlFacade controlFacade, ILogger logger)
    {
        _logger = logger;
        _controlFacade = controlFacade;
    }

    public string Execute(string command)
    {
        int volume;

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
                volume = _controlFacade.Audio.GetVolume();
                volume += 5;
                volume = volume > 100 ? 100 : volume;
                _controlFacade.Audio.SetVolume(volume);
                return volume.ToString();
            case Buttons.VolumeDown:
                volume = _controlFacade.Audio.GetVolume();
                volume -= 5;
                volume = volume < 0 ? 0 : volume;
                _controlFacade.Audio.SetVolume(volume);
                return volume.ToString();
            case Buttons.Darken:
                _controlFacade.Display.Darken();
                break;
            default:
                break;
        }

        return "done";
    }
}