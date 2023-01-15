using NAudio.CoreAudioApi;
using Shared.AudioWrapper;

namespace ControlProviders.Wrappers;

public class MMDeviceWrapper: IMMDevice
{
    public string ID => _device.ID;
    public string DeviceFriendlyName => _device.DeviceFriendlyName;
    public IAudioEndpointVolume AudioEndpointVolume => new AudioEndpointVolumeWrapper(_device.AudioEndpointVolume);

    private readonly MMDevice _device;

    public MMDeviceWrapper(MMDevice device)
    {
        _device = device;
    }
}