using ControlProviders.Interfaces;
using NAudio.CoreAudioApi;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

[assembly: InternalsVisibleTo("UnitTests")]

namespace ControlProviders;

internal static partial class Utils
{
    [GeneratedRegex("[0-9A-F]{8}[-][0-9A-F]{4}[-][0-9A-F]{4}[-][0-9A-F]{4}[-][0-9A-F]{12}", RegexOptions.IgnoreCase)]
    public static partial Regex GuidRegex();
}

public class NAudioWrapper : IAudioControl
{
    private readonly MMDeviceEnumerator _enumerator = new();

    private MMDevice DefaultDevice => _defaultGuid == null
        ? _enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia)
        : Devices.First(x => GetGuid(x.ID) == _defaultGuid);
    private IEnumerable<MMDevice> Devices => _enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);

    private Guid? _defaultGuid = null;

    private static Guid GetGuid(string input) => new(Utils.GuidRegex().Match(input).Value);

    public int GetVolume() => GetVolume(null);

    private int GetVolume(Guid? id)
    {
        return id == null
            ? (int)(DefaultDevice.AudioEndpointVolume.MasterVolumeLevelScalar * 100)
            : (int)(Devices.FirstOrDefault(x => x.ID == id.ToString())
                    ?.AudioEndpointVolume.MasterVolumeLevelScalar * 100 ?? 0);
    }

    public void SetVolume(int volume) => SetVolume(volume, null);

    public void SetVolume(int volume, Guid? id)
    {
        if (volume is < 0 or > 100) throw new ArgumentOutOfRangeException(nameof(volume));

        var device = id == null ? DefaultDevice : Devices.First(x => x.ID == id.ToString());
        device.AudioEndpointVolume.MasterVolumeLevelScalar = (float)volume / 100;
    }


    public void Mute() => DefaultDevice.AudioEndpointVolume.Mute = true;
    public void Unmute() => DefaultDevice.AudioEndpointVolume.Mute = false;

    public bool IsMuted => DefaultDevice.AudioEndpointVolume.Mute;

    public IEnumerable<IAudioDevice> GetAudioDevices()
    {
        return Devices.Select(x =>
            new AudioDevice
            {
                Id = GetGuid(x.ID),
                IsCurrentControlDevice = x.ID == DefaultDevice.ID,
                Name = x.DeviceFriendlyName
            });
    }

    public void SetAudioDevice(Guid id) => _defaultGuid = id;

    public void SetAudioDevice(IAudioDevice device) => SetAudioDevice(device.Id);
}