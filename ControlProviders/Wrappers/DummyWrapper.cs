using Shared.ControlProviders.Devices;
using Shared.ControlProviders.Input;
using Shared.Enums;

namespace ControlProviders.Wrappers;

public class DummyWrapper : IKeyboardInput, IMouseInput, IDisplayInput, IAudioInput
{
    public void SendKey(KeysEnum key, KeyPressMode mode)
    {
    }

    public void SendText(string text)
    {
    }

    public void SendMouseKey(MouseButtons key, KeyPressMode mode)
    {
    }

    public void SendMouseMove(int x, int y)
    {
    }

    public void SendScroll(int scrollAmount)
    {
    }

    public void SetState(MonitorState state)
    {
    }

    public bool IsMute { get; set; }
    public int GetVolume(Guid? id = null)
    {
        return 0;
    }

    public void SetVolume(int volume, Guid? id = null)
    {
    }

    public IEnumerable<IAudioDevice> GetDevices()
    {
        return Enumerable.Empty<IAudioDevice>();
    }

    public void SetCurrentDevice(IAudioDevice device)
    {
    }

    public void SetCurrentDevice(Guid deviceId)
    {
    }
}