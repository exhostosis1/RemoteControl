using NAudio.CoreAudioApi;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using ControlProviders.Interfaces;

[assembly: InternalsVisibleTo("UnitTests")]

namespace ControlProviders;

internal static partial class Utils
{
    [GeneratedRegex("[0-9A-F]{8}[-][0-9A-F]{4}[-][0-9A-F]{4}[-][0-9A-F]{4}[-][0-9A-F]{12}", RegexOptions.IgnoreCase)]
    public static partial Regex GuidRegex();
}

public class NAudioWrapper : IAudioControlProvider
{
    private static readonly MMDeviceEnumerator Enumerator = new();

    private MMDevice _defaultDevice;
    private readonly IEnumerable<MMDevice> _devices;

    public NAudioWrapper()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            throw new Exception("OS not supported");

        _devices = Enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
        _defaultDevice = Enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
    }

    private static Guid GetGuid(string input) => new(Utils.GuidRegex().Match(input).Value);

    public int GetVolume() => GetVolume(null);

    private int GetVolume(Guid? id)
    {
        return id == null
            ? (int)(_defaultDevice.AudioEndpointVolume.MasterVolumeLevelScalar * 100)
            : (int)(_devices.FirstOrDefault(x => x.ID == id.ToString())
                    ?.AudioEndpointVolume.MasterVolumeLevelScalar * 100 ?? 0);
    }

    public void SetVolume(int volume) => SetVolume(volume, null);

    public void SetVolume(int volume, Guid? id)
    {
        if (volume is < 0 or > 100) throw new ArgumentOutOfRangeException(nameof(volume));

        var device = id == null ? _defaultDevice : _devices.First(x => x.ID == id.ToString());
        device.AudioEndpointVolume.MasterVolumeLevelScalar = (float)volume / 100;
    }


    public void Mute() => _defaultDevice.AudioEndpointVolume.Mute = true;
    public void Unmute() => _defaultDevice.AudioEndpointVolume.Mute = false;

    public bool IsMuted => _defaultDevice.AudioEndpointVolume.Mute;

    public IEnumerable<IAudioDevice> GetAudioDevices()
    {
        return _devices.Select(x =>
            new AudioDevice
            {
                Id = GetGuid(x.ID),
                IsCurrentControlDevice = x.ID == _defaultDevice.ID,
                Name = x.DeviceFriendlyName
            });
    }

    public void SetAudioDevice(Guid id)
    {
        _defaultDevice = _devices.First(x => GetGuid(x.ID) == id);
    }

    public void SetAudioDevice(IAudioDevice device) => SetAudioDevice(device.Id);
}