using Shared.ControlProviders.Devices;
using Shared.Enums;
using System;
using System.Collections.Generic;

namespace Shared.ControlProviders;

public interface IDisplayInput
{
    public void SetState(MonitorState state);
}

public interface IAudioInput
{
    public bool IsMute { get; set; }
    public int GetVolume(Guid? id = null);
    public void SetVolume(int volume, Guid? id = null);
    public IEnumerable<IAudioDevice> GetDevices();
    public void SetCurrentDevice(IAudioDevice device);
    public void SetCurrentDevice(Guid deviceId);
}

public interface IKeyboardInput
{
    public void SendKey(KeysEnum key, KeyPressMode mode);
    public void SendText(string text);
}

public interface IMouseInput
{
    public void SendMouseKey(MouseButtons key, KeyPressMode mode);
    public void SendMouseMove(int x, int y);
    public void SendScroll(int scrollAmount);
}