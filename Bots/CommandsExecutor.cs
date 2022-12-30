using Shared.ControlProviders;
using Shared.Enums;
using Shared.Logging.Interfaces;

namespace Bots;

public class CommandsExecutor
{
    private readonly Dictionary<string, KeysEnum> _keys = new()
    {
        {
            Buttons.MediaBack, KeysEnum.MediaPrev
        },
        {
            Buttons.MediaForth, KeysEnum.MediaNext
        },
        {
            Buttons.Pause, KeysEnum.MediaPlayPause
        },
        {
            Buttons.Mute, KeysEnum.Mute
        },
        {
            Buttons.VolumeUp, KeysEnum.VolumeUp
        },
        {
            Buttons.VolumeDown, KeysEnum.VolumeDown
        }
    };

    private readonly ILogger _logger;

    private readonly IKeyboardControlProvider _keyboardControl;

    public CommandsExecutor(IKeyboardControlProvider keyboardControl, ILogger logger)
    {
        _logger = logger;
        _keyboardControl = keyboardControl;
    }

    public void Execute(string command)
    {
        if (_keys.TryGetValue(command, out var key))
        {
            _keyboardControl.KeyPress(key);
        }
    }
}