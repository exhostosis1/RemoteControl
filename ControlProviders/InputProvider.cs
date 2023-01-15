using Shared;
using Shared.ControlProviders;
using Shared.Enums;
using Shared.Logging.Interfaces;

namespace ControlProviders;

public class InputProvider : IDisplayControlProvider, IKeyboardControlProvider, IMouseControlProvider
{
    private readonly ILogger<InputProvider> _logger;
    private readonly IInput _input;

    private const int MouseWheelClickSize = 120;

    public InputProvider(IInput input, ILogger<InputProvider> logger)
    {
        _input = input;
        _logger = logger;
    }

    public void Darken() => _input.SetMonitorInState(MonitorState.MonitorStateOff);

    public void KeyPress(KeysEnum key, KeyPressMode mode = KeyPressMode.Click)
    {
        if (mode.HasFlag(KeyPressMode.Down))
            _input.SendKeyInput(key, false);

        if (mode.HasFlag(KeyPressMode.Up))
            _input.SendKeyInput(key, true);
    }

    public void TextInput(string text)
    {
        foreach (var c in text.ToCharArray())
        {
            _input.SendCharInput(c, false);
            _input.SendCharInput(c, true);
        }
    }

    public void Move(int x, int y) => _input.SendMouseInput(x, y);

    public void ButtonPress(MouseButtons button = MouseButtons.Left, KeyPressMode mode = KeyPressMode.Click)
    {
        if(mode.HasFlag(KeyPressMode.Down))
            _input.SendMouseInput(button, false);

        if (mode.HasFlag(KeyPressMode.Up))
            _input.SendMouseInput(button, true);
    }

    public void Wheel(bool up) => _input.SendScrollInput(up ? MouseWheelClickSize : -MouseWheelClickSize);
}