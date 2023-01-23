using Shared.ControlProviders.Devices;
using System;
using System.Collections.Generic;

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