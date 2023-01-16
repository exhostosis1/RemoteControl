using Shared.ApiControllers;
using Shared.ControlProviders;
using Shared.Enums;
using Shared.Logging.Interfaces;

namespace Bots;

public class CommandsExecutor: ICommandExecutor
{
    private readonly ILogger<CommandsExecutor> _logger;

    private readonly IControlProvider _controlFacade;

    public CommandsExecutor(IControlProvider controlFacade, ILogger<CommandsExecutor> logger)
    {
        _logger = logger;
        _controlFacade = controlFacade;
    }

    public string Execute(string command)
    {
        _logger.LogInfo($"Executing bot command {command}");

        var volume = _controlFacade.GetVolume();

        switch (command)
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
                return volume.ToString();
            case BotButtons.VolumeDown:
                volume -= 5;
                volume = volume < 0 ? 0 : volume;
                _controlFacade.SetVolume(volume);
                return volume.ToString();
            case BotButtons.Darken:
                _controlFacade.DisplayOff();
                break;
            default:
                if (int.TryParse(command, out volume))
                {
                    volume = volume < 0 ? 0 : volume > 100 ? 100 : volume;
                    _controlFacade.SetVolume(volume);
                }
                return "done";
        }

        return "done";
    }
}