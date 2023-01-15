using ControlProviders.Devices;
using Shared.AudioWrapper;
using Shared.ControlProviders;
using Shared.ControlProviders.Devices;
using Shared.Logging.Interfaces;
using System.Text.RegularExpressions;

namespace ControlProviders;

public partial class NAudioProvider: IAudioControlProvider
{
    private IMMDevice _defaultDevice;
    private readonly IEnumerable<IMMDevice> _devices;
    private readonly ILogger<NAudioProvider> _logger;

    [GeneratedRegex("[0-9A-F]{8}[-][0-9A-F]{4}[-][0-9A-F]{4}[-][0-9A-F]{4}[-][0-9A-F]{12}", RegexOptions.IgnoreCase)]
    private static partial Regex GuidRegex();

    public NAudioProvider(IAudioDeviceEnumerator enumerator, ILogger<NAudioProvider> logger)
    {
        _logger = logger;

        _devices = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
        _defaultDevice = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
    }

    private static Guid GetGuid(string input) => new(GuidRegex().Match(input).Value);

    public int GetVolume()
    {
        _logger.LogInfo("Getting volume");
        return (int)(_defaultDevice.AudioEndpointVolume.MasterVolumeLevelScalar * 100);
    }

    public void SetVolume(int volume)
    {
        if (volume is < 0 or > 100) throw new ArgumentOutOfRangeException(nameof(volume));

        _logger.LogInfo($"Setting volume to {volume}");
        _defaultDevice.AudioEndpointVolume.MasterVolumeLevelScalar = (float)volume / 100;
    }

    public void Mute()
    {
        _logger.LogInfo("Muting device");
        _defaultDevice.AudioEndpointVolume.Mute = true;
    }

    public void Unmute()
    {
        _logger.LogInfo("Unmuting device");
        _defaultDevice.AudioEndpointVolume.Mute = false;
    }

    public bool IsMuted => _defaultDevice.AudioEndpointVolume.Mute;

    public IReadOnlyCollection<IAudioDevice> GetDevices()
    {
        _logger.LogInfo("Getting devices");

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
        _logger.LogInfo($"Setting device to {id}");

        _defaultDevice = _devices.First(x => GetGuid(x.ID) == id);
        return GetDevices();
    }
}