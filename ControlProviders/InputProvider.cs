using Shared.ControlProviders;
using Shared.ControlProviders.Devices;
using Shared.Enums;
using Shared.Logging.Interfaces;

namespace ControlProviders;

public class InputProvider : IControlProvider
{
    private readonly ILogger<InputProvider> _logger;
    private readonly IKeyboardInput _keyboardInput;
    private readonly IMouseInput _mouseInput;
    private readonly IDisplayInput _displayInput;
    private readonly IAudioInput _audioInput;

    private const int MouseWheelClickSize = 120;

    public InputProvider(IKeyboardInput keyboardInput, IMouseInput mouseInput, IDisplayInput displayInput, IAudioInput audioInput, ILogger<InputProvider> logger)
    {
        _logger = logger;
        _keyboardInput = keyboardInput;
        _mouseInput = mouseInput;
        _displayInput = displayInput;
        _audioInput = audioInput;
    }

    public int GetVolume() => _audioInput.GetVolume();

    public void SetVolume(int volume) => _audioInput.SetVolume(volume);

    public void Mute() => _audioInput.IsMute = true;

    public void Unmute() => _audioInput.IsMute = false;

    public bool IsMuted => _audioInput.IsMute;
    public IEnumerable<IAudioDevice> GetAudioDevices() => _audioInput.GetDevices();

    public void SetAudioDevice(Guid id) => _audioInput.SetCurrentDevice(id);

    public void DisplayOff() => _displayInput.SetState(MonitorState.MonitorStateOff);

    public void KeyboardKeyPress(KeysEnum key, KeyPressMode mode = KeyPressMode.Click) => _keyboardInput.SendKey(key, mode);

    public void TextInput(string text) => _keyboardInput.SendText(text);

    public void MouseMove(int x, int y) => _mouseInput.SendMouseMove(x, y);

    public void MouseKeyPress(MouseButtons button = MouseButtons.Left, KeyPressMode mode = KeyPressMode.Click) =>
        _mouseInput.SendMouseKey(button, mode);

    public void MouseWheel(bool up) => _mouseInput.SendScroll(up ? MouseWheelClickSize : -MouseWheelClickSize);
}