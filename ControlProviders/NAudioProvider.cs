using System.Text.RegularExpressions;
using ControlProviders.Abstract;
using ControlProviders.Devices;
using NAudio.CoreAudioApi;
using Shared.ControlProviders;
using Shared.ControlProviders.Devices;
using Shared.Logging.Interfaces;

namespace ControlProviders;

public partial class NAudioProvider: BaseProvider, IAudioControlProvider
{
    private MMDevice _defaultDevice;
    private readonly IEnumerable<MMDevice> _devices;

    [GeneratedRegex("[0-9A-F]{8}[-][0-9A-F]{4}[-][0-9A-F]{4}[-][0-9A-F]{4}[-][0-9A-F]{12}", RegexOptions.IgnoreCase)]
    private static partial Regex GuidRegex();

    public NAudioProvider(ILogger logger): base(logger)
    {
        var enumerator = new MMDeviceEnumerator();

        _devices = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
        _defaultDevice = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
    }

    private static Guid GetGuid(string input) => new(GuidRegex().Match(input).Value);

    public int GetVolume()
    {
        Logger.LogInfo("Getting volume");
        return (int)(_defaultDevice.AudioEndpointVolume.MasterVolumeLevelScalar * 100);
    }

    public void SetVolume(int volume)
    {
        Logger.LogInfo($"Setting volume to {volume}");
        _defaultDevice.AudioEndpointVolume.MasterVolumeLevelScalar = (float)volume / 100;
    }

    public void Mute()
    {
        Logger.LogInfo("Muting device");
        _defaultDevice.AudioEndpointVolume.Mute = true;
    }

    public void Unmute()
    {
        Logger.LogInfo("Unmuting device");
        _defaultDevice.AudioEndpointVolume.Mute = false;
    }

    public bool IsMuted => _defaultDevice.AudioEndpointVolume.Mute;

    public IReadOnlyCollection<IAudioDevice> GetDevices()
    {
        Logger.LogInfo("Getting devices");

        return _devices.Select(x =>
            new AudioDevice
            {
                Id = GetGuid(x.ID),
                IsCurrentControlDevice = x.ID == _defaultDevice.ID,
                Name = x.DeviceFriendlyName
            }).ToList();
    }

    public IReadOnlyCollection<IAudioDevice> SetCurrentControlDevice(Guid id)
    {
        Logger.LogInfo($"Setting device to {id}");

        _defaultDevice = _devices.First(x => GetGuid(x.ID) == id);
        return GetDevices();
    }
}