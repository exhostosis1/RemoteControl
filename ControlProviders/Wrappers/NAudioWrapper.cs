using ControlProviders.Devices;
using NAudio.CoreAudioApi;
using Shared;
using Shared.ControlProviders;
using Shared.ControlProviders.Devices;
using Shared.Logging.Interfaces;

namespace ControlProviders.Wrappers;

public class NAudioWrapper: IAudioInput
{
    private static readonly MMDeviceEnumerator Enumerator = new ();

    private MMDevice _defaultDevice;
    private readonly IEnumerable<MMDevice> _devices;

    public NAudioWrapper()
    {
        _devices = Enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
        _defaultDevice = Enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
    }

    private static Guid GetGuid(string input) => new(Utils.GuidRegex().Match(input).Value);

    public int GetVolume(Guid? id = null)
    {
        return id == null
            ? (int)(_defaultDevice.AudioEndpointVolume.MasterVolumeLevelScalar * 100)
            : (int)(_devices.FirstOrDefault(x => x.ID == id.ToString())?.AudioEndpointVolume.MasterVolumeLevelScalar *
                100 ?? 0);
    }

    public void SetVolume(int volume, Guid? id = null)
    {
        if (volume is < 0 or > 100) throw new ArgumentOutOfRangeException(nameof(volume));

        var device = id == null ? _defaultDevice : _devices.First(x => x.ID == id.ToString());
        device.AudioEndpointVolume.MasterVolumeLevelScalar = (float)volume / 100;
    }


    public bool IsMute
    {
        get => _defaultDevice.AudioEndpointVolume.Mute;
        set => _defaultDevice.AudioEndpointVolume.Mute = value;
    }

    public IEnumerable<IAudioDevice> GetDevices()
    {
        return _devices.Select(x =>
            new AudioDevice
            {
                Id = GetGuid(x.ID),
                IsCurrentControlDevice = x.ID == _defaultDevice.ID,
                Name = x.DeviceFriendlyName
            });
    }

    public void SetCurrentDevice(Guid id)
    {
        _defaultDevice = _devices.First(x => GetGuid(x.ID) == id);
    }

    public void SetCurrentDevice(IAudioDevice device) => SetCurrentDevice(device.Id);
}