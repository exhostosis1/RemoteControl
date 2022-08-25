using System;
using System.Collections.Generic;
using Shared.ControlProviders.Devices;

namespace Shared.ControlProviders
{
    public interface IAudioControlProvider
    {
        public int GetVolume();
        public void SetVolume(int volume);
        void Mute();
        void Unmute();
        IReadOnlyCollection<IAudioDevice> GetDevices();
        IReadOnlyCollection<IAudioDevice> SetCurrentControlDevice(Guid id);
    }
}
