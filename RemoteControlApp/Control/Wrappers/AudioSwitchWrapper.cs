using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;
using Shared.Interfaces.Control;

namespace RemoteControlApp.Control.Wrappers
{
    internal class AudioDevice : IAudioDevice
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public bool IsActive { get; set; }
        public bool IsDefault { get; set; }
    }
    public class AudioSwitchWrapper: IAudioControl
    {
        private IDevice _audioDevice;
        private readonly IAudioController _audioController = new CoreAudioController();

        public int Volume
        {
            get => (int)_audioDevice.Volume;
            set => _audioDevice.SetVolumeAsync(value);
        }

        public AudioSwitchWrapper()
        {
            _audioDevice = _audioController.GetDefaultDevice(DeviceType.Playback, Role.Multimedia);
        }

        public IReadOnlyCollection<IAudioDevice> GetDevices()
        {
            return _audioController.GetDevices(DeviceType.Playback, DeviceState.Active).Select(x => new AudioDevice
            {
                Id = x.Id,
                IsDefault = x.IsDefaultDevice,
                IsActive = x.Id == _audioDevice.Id,
                Name = x.FullName
            }).ToList();
        }

        public void SetDevice(Guid id)
        {
            _audioDevice = _audioController.GetDevice(id);
        }

        public void Mute(bool mute) => _audioDevice.SetMuteAsync(mute);
    }
}
