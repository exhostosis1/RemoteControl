using System;
using System.Collections.Generic;
using Shared.ControlProviders.Devices;

namespace Shared.ControlProviders.Input;

public interface IAudioInput
{
    public bool IsMute { get; set; }
    public int GetVolume(Guid? id = null);
    public void SetVolume(int volume, Guid? id = null);
    public IEnumerable<IAudioDevice> GetDevices();
    public void SetCurrentDevice(IAudioDevice device);
    public void SetCurrentDevice(Guid deviceId);
}