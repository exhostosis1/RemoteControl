using NAudio.CoreAudioApi;
using Shared.AudioWrapper;
using DataFlow = Shared.AudioWrapper.DataFlow;
using DeviceState = Shared.AudioWrapper.DeviceState;
using Role = Shared.AudioWrapper.Role;

namespace ControlProviders.Wrappers;

public class NAudioWrapper: IAudioDeviceEnumerator
{
    private static readonly MMDeviceEnumerator Enumerator = new ();

    public IEnumerable<IMMDevice> EnumerateAudioEndPoints(DataFlow flow, DeviceState state) =>
        Enumerator.EnumerateAudioEndPoints((NAudio.CoreAudioApi.DataFlow)flow, (NAudio.CoreAudioApi.DeviceState)state).Select(x => new MMDeviceWrapper(x));

    public IMMDevice GetDefaultAudioEndpoint(DataFlow flow, Role role) => new MMDeviceWrapper(
        Enumerator.GetDefaultAudioEndpoint((NAudio.CoreAudioApi.DataFlow)flow, (NAudio.CoreAudioApi.Role)role));
}