using Shared.ControlProviders.Devices;
using Shared.ControlProviders.Input;
using Shared.ControlProviders.Provider;
using Shared.Enums;
using Shared.Logging.Interfaces;

namespace ControlProviders;

public class InputProvider(IKeyboardInput keyboardInput, IMouseInput mouseInput, IDisplayInput displayInput, IAudioInput audioInput) : IGeneralControlProvider
{
    private const int MouseWheelClickSize = 120;

    public int GetVolume() => audioInput.GetVolume();

    public void SetVolume(int volume) => audioInput.SetVolume(volume);

    public void Mute() => audioInput.IsMute = true;

    public void Unmute() => audioInput.IsMute = false;

    public bool IsMuted => audioInput.IsMute;
    public IEnumerable<IAudioDevice> GetAudioDevices() => audioInput.GetDevices();

    public void SetAudioDevice(Guid id) => audioInput.SetCurrentDevice(id);

    public void DisplayOff() => displayInput.SetState(MonitorState.MonitorStateOff);

    public void KeyboardKeyPress(KeysEnum key, KeyPressMode mode = KeyPressMode.Click) => keyboardInput.SendKey(key, mode);

    public void TextInput(string text) => keyboardInput.SendText(text);

    public void MouseMove(int x, int y) => mouseInput.SendMouseMove(x, y);

    public void MouseKeyPress(MouseButtons button = MouseButtons.Left, KeyPressMode mode = KeyPressMode.Click) =>
        mouseInput.SendMouseKey(button, mode);

    public void MouseWheel(bool up) => mouseInput.SendScroll(up ? MouseWheelClickSize : -MouseWheelClickSize);
}