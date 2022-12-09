using Shared.ControlProviders;
using Shared.Enums;

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

    private readonly IKeyboardControlProvider _keyboardControl;

    public CommandsExecutor(IKeyboardControlProvider keyboardControl)
    {
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