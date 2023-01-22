using System;
using System.Collections.Generic;
using Shared.ControlProviders.Devices;

namespace Shared.ControlProviders.Provider;

public interface IAudioControlProvider
{
    public int GetVolume();
    public void SetVolume(int volume);
    void Mute();
    void Unmute();
    bool IsMuted { get; }
    IEnumerable<IAudioDevice> GetAudioDevices();
    void SetAudioDevice(Guid id);
}