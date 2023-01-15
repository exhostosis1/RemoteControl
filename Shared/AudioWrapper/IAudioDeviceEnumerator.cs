using System.Collections.Generic;

namespace Shared.AudioWrapper;

public interface IAudioDeviceEnumerator
{
    public IEnumerable<IMMDevice> EnumerateAudioEndPoints(DataFlow flow, DeviceState state);
    public IMMDevice GetDefaultAudioEndpoint(DataFlow flow, Role role);
}