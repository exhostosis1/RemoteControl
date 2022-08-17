using NAudio.CoreAudioApi;
using Shared.Interfaces.Control;
using System.Text.RegularExpressions;
using Shared.Interfaces.Logging;

namespace Control.Wrappers
{
    public class NAudioWrapper: BaseWrapper, IAudioControl
    {
        private MMDevice _defaultDevice;
        private readonly IEnumerable<MMDevice> _devices;

        private static readonly Regex GuidRegex =
            new ("[0-9A-F]{8}[-][0-9A-F]{4}[-][0-9A-F]{4}[-][0-9A-F]{4}[-][0-9A-F]{12}", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public NAudioWrapper(ILogger logger): base(logger)
        {
            var enumerator = new MMDeviceEnumerator();

            _devices = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
            _defaultDevice = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
        }

        private static Guid GetGuid(string input) => new(GuidRegex.Match(input).Value);

        public int GetVolume() => (int)(_defaultDevice.AudioEndpointVolume.MasterVolumeLevelScalar * 100);

        public void SetVolume(int volume) => _defaultDevice.AudioEndpointVolume.MasterVolumeLevelScalar = (float)volume / 100;

        public void Mute(bool mute)
        {
            _defaultDevice.AudioEndpointVolume.Mute = mute;
        }

        public IReadOnlyCollection<IAudioDevice> GetDevices()
        {
            return _devices.Select(x =>
                new AudioDevice
                {
                    Id = GetGuid(x.ID),
                    IsCurrentControlDevice = x.ID == _defaultDevice.ID,
                    Name = x.FriendlyName
                }).ToList();
        }

        public IReadOnlyCollection<IAudioDevice> SetCurrentControlDevice(Guid id)
        {
            _defaultDevice = _devices.First(x => GetGuid(x.ID) == id);
            return GetDevices();
        }
    }
}
