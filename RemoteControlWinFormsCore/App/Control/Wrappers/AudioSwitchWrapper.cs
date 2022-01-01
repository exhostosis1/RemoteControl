using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;
using RemoteControl.App.Control.Interfaces;

namespace RemoteControl.App.Control.Wrappers
{
    internal class AudioDevice : IAudioDevice
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public bool IsActive { get; set; }
    }
    internal class AudioSwitchWrapper: IControlAudio
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
            //_audioController.AudioDeviceChanged
            //    .When(x => x.ChangedType == DeviceChangedType.DefaultChanged && x.Device.IsPlaybackDevice)
            //    .Subscribe(x => _audioDevice = x.Device);
        }

        public IEnumerable<IAudioDevice> GetDevices()
        {
            return _audioController.GetDevices(DeviceType.Playback, DeviceState.Active).Select(x => new AudioDevice
            {
                Id = x.Id,
                IsActive = x.Id == _audioDevice.Id,
                Name = x.FullName
            });
        }

        public void SetDevice(Guid id)
        {
            _audioDevice = _audioController.GetDevice(id);
        }

        public void Mute(bool mute) => _audioDevice.SetMuteAsync(mute);
    }
}
